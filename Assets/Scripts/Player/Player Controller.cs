using Survivor.Core;
using System;
using UnityEngine;

namespace Survivor.Player
{
    [RequireComponent(typeof(PlayerAnimator))]
    public class PlayerController : MonoBehaviour, ISaveable
    {
        [Header("Dependencies")]
        [SerializeField] private InputProvider  m_InputProvider;
        [SerializeField] private DialogSystem   m_DialogSystem;
        [SerializeField] private PlayerBehavior m_Behavior;
        [SerializeField] private BoxCollider    m_WeaponCollider;


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
            m_Animator = GetComponent<PlayerAnimator>();

            if(SaveSystem.TryFindSaveData(SaveKey, out PlayerSavedData data))
            {
                m_Health  = new PlayerStat(data.Health , m_Health.Maximum , PlayerStatType.Health);
                m_Hunger  = new PlayerStat(data.Hunger , m_Hunger.Maximum , PlayerStatType.Hunger);
                m_Stamina = new PlayerStat(data.Stamina, m_Stamina.Maximum, PlayerStatType.Stamina);

                transform.position = data.Position;
            }
            SaveSystem.RegisterSaveable(this);
        }

        private void Update()
        {
            if(GameController.IsGameMode(GameController.GameMode.Gameplay))
            {
                var gameMode = UpdateGameMode();
                if(gameMode == GameController.GameMode.Gameplay)
                {
                    bool isAttacking = m_Animator.IsClipPlaying(PlayerAnimator.AttackClip);

                    //
                    // Update the player's movement (if we are not in the attack state)
                    //

                    if(!isAttacking)
                    {
                        if(TryUpdatePlayerMovement(transform, m_InputProvider.Game.Move, m_Behavior.MoveSpeed, out Vector3 position, out Quaternion rotation))
                        {
                            transform.SetPositionAndRotation(position, rotation);
                            m_Animator.SetParam(true, PlayerAnimator.IsWalking);
                        }
                        else
                        {
                            m_Animator.SetParam(false, PlayerAnimator.IsWalking);
                        }
                    }

                    //
                    // Try to attack if the input is pressed.
                    //

                    if(m_InputProvider.Game.IsAttacking)
                    {
                        var attackState = TryAttack(Stamina, m_Behavior.AttackCost, m_Behavior.AttackSpeed, m_LastAttackTime);
                        if(attackState == AttackState.Success)
                        {
                            m_LastAttackTime = Time.time;
                            m_Stamina       -= m_Behavior.AttackCost;

                            m_Animator.SetParam(true, PlayerAnimator.Attacked);
                        }
                        else if(attackState == AttackState.OnCooldown)
                        {

                        }
                        else if(attackState == AttackState.NotEnoughStamina)
                        {

                        }
                    }
                    
                    //
                    // Update the player's stat
                    //

                    m_Stamina += m_Behavior.StaminaIncreaseRate;
                    m_Hunger  -= m_Behavior.HungerDecreaseRate;

                    //
                    // Update the weapon's hitbox.
                    //

                    m_WeaponCollider.enabled = isAttacking;
                }
                else
                {
                    if (gameMode == GameController.GameMode.Finished)
                    {
                        m_Animator.SetParam(PlayerAnimator.Died);
                    }

                    GameController.PushGameMode(gameMode);
                }
            }
        }

        private GameController.GameMode UpdateGameMode()
        {
            Debug.Assert(GameController.IsGameMode(GameController.GameMode.Gameplay));

            var result = GameController.GameMode.Gameplay;

            if(m_Health.Current <= 0.0f || m_Hunger.Current <= 0.0f)
            {
                result = GameController.GameMode.Finished;
            }
            else if(m_InputProvider.Always.IsPauseMenuToggled)
            {
                result = GameController.GameMode.Paused;
            }
            else if(m_InputProvider.Always.IsInventoryToggled)
            {
                result = GameController.GameMode.Inventory;
            }
            else if (m_InputProvider.Game.IsInteracting)
            {
                var dialog = TryFindBestDialog(transform.position, 0.0f);
                if(dialog)
                {
                    if(m_DialogSystem.TryEnterDialog(dialog))
                    {
                        result = GameController.GameMode.Dialogue;
                    }
                }
            }

            return result;
        }

        private static bool TryUpdatePlayerMovement(Transform transform, Vector2 moveInput, float moveSpeed, out Vector3 position, out Quaternion rotation)
        {
            bool isMoving = moveInput.x != 0.0f || moveInput.y != 0.0f;
            
            if(isMoving)
            { 
                var cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                var cameraRight   = Vector3.ProjectOnPlane(Camera.main.transform.right  , Vector3.up);
                var lookDirection = Vector3.zero;

                if(moveInput.x > 0.0f)
                {
                    lookDirection += cameraRight;
                }
                else if (moveInput.x < 0.0f)
                {
                    lookDirection -= cameraRight;
                }

                if(moveInput.y > 0.0f)
                {
                    lookDirection += cameraForward;
                }
                else if(moveInput.y < 0.0f)
                {
                    lookDirection -= cameraForward;
                }

                var finalLookDirection = lookDirection.normalized;
                rotation = Math.LookAt(transform.forward, finalLookDirection, Mathf.PI * 2.0f);

                var translationX = moveInput.x * moveSpeed * Time.deltaTime * cameraRight;
                var translationZ = moveInput.y * moveSpeed * Time.deltaTime * cameraForward;
                position = (transform.position + (translationX + translationZ));
            }
            else
            {
                position = default;
                rotation = default;
            }

            return isMoving;
        }

        private static Dialog TryFindBestDialog(Vector3 playerPosition, float range)
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

        //
        // Attacking
        //

        private enum AttackState
        {
            Success = 1,
            OnCooldown = 2,
            NotEnoughStamina = 3,
        }

        private float m_LastAttackTime;

        private static AttackState TryAttack(PlayerStat stamina, float attackCost, float attackSpeed, float lastAttackTime)
        {
            AttackState result;

            if (stamina.Current < attackCost)
            {
                result = AttackState.NotEnoughStamina;
            }
            else
            {
                float currentTime         = Time.time;
                float timeSinceLastAttack = currentTime - lastAttackTime;

                if (timeSinceLastAttack >= attackSpeed)
                {
                    result = AttackState.Success;
                }
                else
                {
                    result = AttackState.OnCooldown;
                }
            }
            
            return result;
        }


        //
        // Interacting
        //

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

    
    [Serializable]
    public enum PlayerStatType
    {
        Health  = 0,
        Hunger  = 1,
        Stamina = 2,
    }

    [Serializable]
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