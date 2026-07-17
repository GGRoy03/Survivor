using UnityEngine;

using Survivor.Player;
using System;

namespace Survivor.Enemy
{
    //
    // TODO:
    // Probably should just be a scriptable object.
    //

    [Serializable]
    public struct Behavior
    {
        [field: SerializeField] public float ChaseRange   { get; private set; }
        [field: SerializeField] public float MoveSpeed    { get; private set; }
        [field: SerializeField] public float AttackRange  { get; private set; }
        [field: SerializeField] public float AttackDamage { get; private set; }
        [field: SerializeField] public float AttackSpeed  { get; private set; }

        [field: SerializeField] public int   ChanceToEnterDefendState { get; private set; }
        [field: SerializeField] public int   ChanceToLeaveDefendState { get; private set; }


    }

    [RequireComponent(typeof(EnemyAnimator))]
    public class EnemyController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private EnemyAnimator m_Animator;
        private PlayerController m_PlayerController;

        [Header("Setup")]
        [SerializeField] private Behavior m_Behavior;

        private EnemyState m_CurrentState;
        public EnemyIdle   EnemyIdleState { get; private set; }
        public EnemyChase  EnemyChaseState { get; private set; }
        public EnemyAttack EnemyAttackState { get; private set; }
        public EnemyDefend EnemyDefendState { get; private set; }
        public EnemyDead   EnemyDeadState   { get; private set; }


        void Start()
        {
            EnemyIdleState   = new(m_PlayerController, this);
            EnemyChaseState  = new(m_PlayerController, this);
            EnemyAttackState = new();
            EnemyDefendState = new();
            EnemyDeadState   = new();

            m_CurrentState     = EnemyIdleState;
            m_PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        void Update()
        {
            if(m_CurrentState != null)
            {
                var animationInfo = m_CurrentState.OnUpdate(m_Behavior, m_PlayerController, this);
                if(m_Animator != null)
                {
                    m_Animator.Animate(animationInfo);
                }
            }
        }

        public void ChangeState(EnemyState nextState)
        {
            m_CurrentState = nextState;
        }
    }

    public abstract class EnemyState
    {
        public abstract AnimationInfo OnUpdate(Behavior behavior, PlayerController player, EnemyController controller);
    }
}