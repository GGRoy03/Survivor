using Survivor.Audio;
using Survivor.Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Survivor.Enemy
{
    [RequireComponent(typeof(EnemyAnimator))]
    public class Cactoro : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private CactoroBehavior m_Behavior;

        //
        // Unity Hooks
        //

        private CactoroState     m_CurrentState;
        private EnemyAnimator    m_Animator;
        private PlayerController m_Player;

        public void Awake()
        {
            //
            // Create the states
            //

            Idle   = gameObject.AddComponent<CactoroIdle>();
            Chase  = gameObject.AddComponent<CactoroChase>();
            Attack = gameObject.AddComponent<CactoroAttack>();
            Defend = gameObject.AddComponent<CactoroDefend>();
            Dead   = gameObject.AddComponent<CactoroDead>();

            //
            // Bind the dependencies
            //

            m_Player   = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            m_Animator = GetComponent<EnemyAnimator>();

            //
            // Set the start state.
            //

            m_CurrentState = Idle;
        }

        public void Update()
        {
            if(m_CurrentState != null)
            {
                m_CurrentState.OnUpdate(this, m_Behavior, m_Animator, m_Player, AudioSystem.Instance);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player Weapon"))
            {
                if (m_CurrentState != null)
                {
                    m_CurrentState.OnAttacked(this);
                }          
            }
        }


        //
        // State-Machine Interface
        //

        public CactoroIdle   Idle   { get; private set; }
        public CactoroChase  Chase  { get; private set; }
        public CactoroDefend Defend { get; private set; }
        public CactoroAttack Attack { get; private set; }
        public CactoroDead   Dead   { get; private set; }

        public void ChangeState(CactoroState state)
        {
            m_CurrentState = state;
        }

    }

    public abstract class CactoroState : MonoBehaviour
    {
        public abstract void OnUpdate(Cactoro cactoro, CactoroBehavior behavior, EnemyAnimator animator, PlayerController player, AudioSystem audio);
        public abstract void OnAttacked(Cactoro cactoro);
    }
}