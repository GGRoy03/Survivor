using UnityEngine;

namespace Survivor.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private DialogSystem        m_DialogSystem; // TODO: Singleton
        [SerializeField] private PlayerInputProvider m_InputProvider;
        [SerializeField] private PlayerAnimator      m_Animator;

        [Header("Behavior")]
        [SerializeField] private PlayerBehavior m_Behavior;

        [Header("Stats")]
        [SerializeField] private PlayerStat m_Health;
        [SerializeField] private PlayerStat m_Stamina;
        [SerializeField] private PlayerStat m_Hunger;

        private PlayerState m_CurrentState;

        //
        // Unity Hooks
        //

        private void Start()
        {
            ChangeState(new PlayerInWorld());
        }

        private void Update()
        {
            if (m_CurrentState != null)
            {
                m_CurrentState.OnUpdate(m_InputProvider, m_Behavior, m_DialogSystem, this);
            }


            m_Animator.Animate(new AnimationInfo());
        }

        //
        // Property Hooks
        //

        public PlayerStat Health
        {
            get => m_Health;
            set => m_Health = value;
        }

        public PlayerStat Hunger
        {
            get => m_Hunger;
            set => m_Hunger = value;
        }

        public PlayerStat Stamina
        {
            get => m_Stamina;
            set => m_Stamina = value;
        }

        //
        // State-Machine Hooks
        //

        public void ChangeState(PlayerState nextState)
        {
            if(nextState != m_CurrentState)
            {
                if (m_InputProvider != null)
                {
                    m_InputProvider.SetInputState(m_CurrentState, false);
                    m_InputProvider.SetInputState(nextState, true);
                }

                m_CurrentState = nextState;
                m_CurrentState.OnEnter(this);
            }
        }
    }

    /// <summary>
    /// Represents the referenced stat from a PlayerStat structure
    /// </summary>

    [System.Serializable]
    public enum PlayerStatType
    {
        Health  = 0,
        Hunger  = 1,
        Stamina = 2,
    }

    /// <summary>
    /// Bundle of player stat with small helpers.
    /// </summary>

    [System.Serializable]
    public struct PlayerStat
    {
        [field: SerializeField] public float          Current {get; private set;}
        [field: SerializeField] public float          Maximum {get; private set;}
        [field: SerializeField] public PlayerStatType Type    {get; private set;}

        public PlayerStat(float value, float max, PlayerStatType type)
        {
            Maximum = max;
            Current = Mathf.Clamp(value, 0.0f, max);
            Type    = type;
        }

        public static PlayerStat operator +(PlayerStat stat, float amount)
        {
            return new PlayerStat(stat.Current + amount, stat.Maximum, stat.Type);
        }

        public static PlayerStat operator -(PlayerStat stat, float amount)
        {
            return new PlayerStat(stat.Current - amount, stat.Maximum, stat.Type);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct PlayerBehavior
    {
        [field: SerializeField] public float InteractRange { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float AttackStaminaCost { get; private set; }
        [field: SerializeField] public float StaminaIncreaseRate { get; private set; }
        [field: SerializeField] public float HungerDecreaseRate { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>

    public abstract class PlayerState
    {
        public abstract void OnEnter(PlayerController controller);
        public abstract void OnUpdate(PlayerInputProvider inputs, PlayerBehavior behavior, DialogSystem dialog, PlayerController controller);

        public abstract AnimationInfo OnAnimate();
    }
}