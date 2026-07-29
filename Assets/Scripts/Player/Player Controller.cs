using Survivor.Audio;
using Survivor.Core;
using Survivor.Inventory;
using System;
using UnityEngine;

namespace Survivor.Player
{
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerController : MonoBehaviour, ISaveable
    {
        [Header("Dependencies")]
        [SerializeField] private InputProvider m_InputProvider;
        [SerializeField] private DialogSystem  m_DialogSystem;
        [SerializeField] private AudioSystem   m_AudioSystem;

        //
        // Stats
        //

        [SerializeField] private PlayerStat m_Health;
        [SerializeField] private PlayerStat m_Stamina;
        [SerializeField] private PlayerStat m_Hunger;

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
        // Unity Hooks
        //

        private PlayerAnimator m_Animator;

        public bool AttackedWithoutStamnia { get; private set; }

        private void Awake()
        {
            //
            // Query the local dependencies
            //

            m_Animator = GetComponent<PlayerAnimator>();

            //
            // Setup the player's load state.
            //

            if(SaveSystem.TryFindSaveData(SaveKey, out PlayerSavedData data))
            {
                m_Health = new PlayerStat(data.Health , m_Health.Maximum , PlayerStatType.Health);
                m_Hunger = new PlayerStat(data.Hunger , m_Hunger.Maximum , PlayerStatType.Hunger);
                m_Health = new PlayerStat(data.Stamina, m_Stamina.Maximum, PlayerStatType.Stamina);

                transform.position = data.Position;
            }
            SaveSystem.RegisterSaveable(this);
        }

        private void Update()
        {
            if(GameController.IsGameMode(GameController.GameMode.Gameplay))
            {
                bool isPauseMenuActivated = m_InputProvider.Always.IsPauseMenuToggled;
                if(isPauseMenuActivated)
                {
                    GameController.PushGameMode(GameController.GameMode.Paused);
                }

                bool isInventoryActivated = m_InputProvider.Always.IsInventoryToggled;
                if(isInventoryActivated)
                {
                    GameController.PushGameMode(GameController.GameMode.Inventory);
                }

                bool isAttacking = m_InputProvider.World.IsAttacking;
                if(isAttacking)
                {
                    var attackState = GetAttackState(0.0f, 0.0f);
                    switch(attackState)
                    {
                        case AttackState.Success:
                        {

                        } break;

                        case AttackState.OnCooldown:
                        {
                            
                        } break;

                        case AttackState.NotEnoughStamina:
                        {
                            AttackedWithoutStamnia = true;
                        } break;
                    }
                }

                bool isInteracting = m_InputProvider.World.IsInteracting;
                if(isInteracting)
                {
                    var dialog = TryFindBestDialog(transform.position, 0.0f);
                    if(dialog != null)
                    {
                        if(m_DialogSystem.TryEnterDialog(dialog))
                        {
                            GameController.PushGameMode(GameController.GameMode.Dialogue);
                        }
                    }
                }

                if(m_Health.Current <= 0.0f)
                {
                    GameController.PushGameMode(GameController.GameMode.Finished);
                }

                if(m_Hunger.Current <= 0.0f)
                {
                    GameController.PushGameMode(GameController.GameMode.Finished);
                }

                SetInteractPromptVisibility(transform.position, 0.0f, 0.0f);
            }
        }

        //
        // Attacking
        //

        private enum AttackState
        {
            Success = 0,
            OnCooldown = 1,
            NotEnoughStamina = 2,
        }

        private float m_LastAttackTime;

        private AttackState GetAttackState(float attackSpeed, float attackStaminaCost)
        {
            AttackState result = AttackState.OnCooldown;

            if (Stamina.Current < attackStaminaCost)
            {
                result = AttackState.NotEnoughStamina;
            }
            else
            {
                float currentTime         = Time.time;
                float timeSinceLastAttack = currentTime - m_LastAttackTime;
                if (timeSinceLastAttack >= attackSpeed)
                {
                    m_LastAttackTime = timeSinceLastAttack;
                    result           = AttackState.Success;
                }
            }

            return result;
        }

        //
        // Interacting
        //

        private Dialog TryFindBestDialog(Vector3 playerPosition, float range)
        {
            Dialog bestDialog    = null;
            float  closestDialog = float.MaxValue;

            var iterator = new PhysicsQueryIterator("NPC", range, playerPosition);
            while(iterator.FindNext(out DialogItem dialogItem))
            {
                Vector3 dialogItemPosition  = dialogItem.transform.position;
                float   sqrDistanceToPlayer = Vector3.SqrMagnitude(playerPosition - dialogItemPosition);

                if(sqrDistanceToPlayer < closestDialog)
                {
                    closestDialog = sqrDistanceToPlayer;
                    bestDialog    = dialogItem.Dialog;
                }
            }
    
            return bestDialog;
        }

        private void SetInteractPromptVisibility(Vector3 playerPosition, float lookRange, float validRange)
        {
            var iterator = new PhysicsQueryIterator("Prompt", lookRange, playerPosition);
            while(iterator.FindNext(out InteractPrompt prompt))
            {
                Vector3 promptPosition      = prompt.transform.position;
                float   sqrDistanceToPlayer = Vector3.SqrMagnitude(playerPosition - promptPosition);

                if(sqrDistanceToPlayer > (validRange * validRange))
                {
                    prompt.SetVisibility(false);
                }
                else
                {
                    prompt.SetVisibility(true);
                }
            }
        }

        //
        // Saving Interface
        //

        [System.Serializable]
        private struct PlayerSavedData
        {
            public float   Health;
            public float   Hunger;
            public float   Stamina;
            public Vector3 Position;
        }

        public int SaveKey => SaveSystem.StringKeyToIntKey("Player");

        public string SaveState()
        {
            string result = SaveSystem.AsSaveData(new PlayerSavedData()
            {
                Health   = m_Health.Current,
                Hunger   = m_Hunger.Current,
                Stamina  = m_Stamina.Current,
                Position = transform.position,
            });
            
            return result;
        }
    }

    
    [System.Serializable]
    public enum PlayerStatType
    {
        Health  = 0,
        Hunger  = 1,
        Stamina = 2,
    }

    [System.Serializable]
    public struct PlayerStat
    {
        [field: SerializeField] public float Current { get; private set; }
        [field: SerializeField] public float Maximum { get; private set; }
        [field: SerializeField] public PlayerStatType Type { get; private set; }

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
}