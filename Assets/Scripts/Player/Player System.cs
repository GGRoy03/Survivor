using UnityEngine;

namespace Survivor.Player
{
    public enum PlayerState
    {
        World  = 0,
        Dialog = 1,
    }

    public class PlayerSystem : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private float m_InteractionRange;

        [Header("Stats")]
        [SerializeField] private float m_MaxHealth;
        [SerializeField] private float m_MaxStamina;
        [SerializeField] private float m_MaxHunger;
        [SerializeField] private float m_HungerDecreaseRate;

        private float m_CurrentHealth;
        private float m_CurrentStamina;
        private float m_CurrentHunger;

        [Header("Dependencies")]
        [SerializeField] private DialogSystem        m_DialogSystem;
        [SerializeField] private PlayerInputProvider m_InputProvider;

        private Camera      m_Camera;
        private PlayerState m_CurrentState;

        [Header("Movement")]
        [SerializeField] private float m_PlayerMoveSpeed;

        public float Health      => m_CurrentHealth;
        public float Hunger      => m_CurrentHunger;
        public float Stamina     => m_CurrentStamina;
        public float MaxHealth   => m_MaxHealth;
        public float MaxStamina  => m_MaxStamina;
        public float MaxHunger   => m_MaxHunger;
        public bool  CanInteract => (m_CurrentState == PlayerState.World && (m_DialogSystem != null && m_DialogSystem.CanChangeDialogState));


        private void Start()
        {
            m_CurrentState  = PlayerState.World;
            m_Camera        = Camera.main;

            m_CurrentHealth  = m_MaxHealth;
            m_CurrentStamina = m_MaxStamina;
            m_CurrentHunger  = m_MaxHunger;
        }

        private void Update()
        {
            switch (m_CurrentState)
            {
                case PlayerState.World:
                {
                    var stateAfter = UpdatePlayerInWorld(m_InputProvider.MoveInput, m_InputProvider.IsInteracting);
                    if (stateAfter != m_CurrentState)
                    {
                        ChangePlayerState(m_CurrentState, stateAfter);
                    }
                } break;

                case PlayerState.Dialog:
                {
                    var stateAfter = UpdatePlayerInDialog(m_InputProvider.IsSkippingDialog);
                    if(stateAfter != m_CurrentState)
                    {
                        ChangePlayerState(m_CurrentState, stateAfter);
                    }          
                } break;
            }
        }

        private void ChangePlayerState(PlayerState prevState, PlayerState nextState)
        {
            m_CurrentState = nextState;

            if(m_InputProvider != null)
            {
                m_InputProvider.SetInputState(prevState, false);
                m_InputProvider.SetInputState(nextState, true);
            }
        }

        private Dialog TryFindBestDialog()
        {
            var interactLayer        = LayerMask.GetMask("NPC");
            var interactSqrDistance  = m_InteractionRange * m_InteractionRange;
            var interactColliders    = Physics.OverlapSphere(transform.position, m_InteractionRange, interactLayer);
            var canChangeDialogState = m_DialogSystem != null && m_DialogSystem.CanChangeDialogState;

            Dialog bestDialog = null;
            float closestSqrDist = float.MaxValue;
            foreach (var collider in interactColliders)
            {
                var colliderObject = collider.gameObject;

                if (canChangeDialogState)
                {
                    if (colliderObject.TryGetComponent(out DialogItem dialogItem))
                    {
                        float sqrDistanceToPlayer = Vector3.SqrMagnitude(transform.position - collider.transform.position);
                        if (sqrDistanceToPlayer < closestSqrDist)
                        {
                            closestSqrDist = sqrDistanceToPlayer;
                            bestDialog = dialogItem.Dialog;
                        }
                    }
                }
            }

            return bestDialog;
        }

        private void SetInteractPromptVisiblity(Dialog bestDialog)
        {
            //
            // If we found a dialog this frame
            // or we are exiting the cinematic (canChangeDialogState == false), we want to
            // hide the prompt.
            //
            // NOTE:
            // What happens when the player teleports? Or is displaced very rapidly? We will miss
            // out on some prompts. It's not the case right now, so we stick with this for simplicity.
            //

            var interactLayer        = LayerMask.GetMask("Prompt");
            var interactSqrDistance  = m_InteractionRange * m_InteractionRange;
            var interactColliders    = Physics.OverlapSphere(transform.position, m_InteractionRange * 1.5f, interactLayer);
            var canChangeDialogState = m_DialogSystem != null && m_DialogSystem.CanChangeDialogState;

            foreach (var collider in interactColliders)
            {
                if (collider.gameObject.TryGetComponent<InteractPrompt>(out var prompt))
                {
                    var colliderPosition    = collider.transform.position;
                    var colliderSqrDistance = Vector3.SqrMagnitude(colliderPosition - transform.position);

                    if (bestDialog || !canChangeDialogState)
                    {
                        prompt.SetVisibility(false);
                    }
                    else
                    {
                        bool isInRange = colliderSqrDistance <= interactSqrDistance;
                        prompt.SetVisibility(isInRange);
                    }
                }
            }
        }

        //
        // NOTE;
        // x) Awfully formatted code.
        //

        private PlayerState UpdatePlayerInWorld(Vector2 moveInput, bool isTryingToInteract)
        {
            PlayerState stateAfterFrame = PlayerState.World;

            Vector3 cameraForward = new(m_Camera.transform.forward.x, 0.0f, m_Camera.transform.forward.z);
            Vector3 cameraRight   = new(m_Camera.transform.right.x  , 0.0f, m_Camera.transform.right.z);
            Vector3 moveDirection = moveInput.x * cameraRight + moveInput.y * cameraForward;
            float   moveSpeed     = m_PlayerMoveSpeed * Time.deltaTime;
            Vector3 moveAmount    = moveSpeed * moveDirection;

            transform.Translate(moveAmount);

            Dialog bestDialog = null;
            if(isTryingToInteract)
            {
                bestDialog = TryFindBestDialog();
            }

            if(bestDialog)
            {
                m_DialogSystem.EnterDialog(bestDialog);
                stateAfterFrame = PlayerState.Dialog;
            }

            SetInteractPromptVisiblity(bestDialog);

            m_CurrentHunger -= m_HungerDecreaseRate;

            return stateAfterFrame;
        }

        private PlayerState UpdatePlayerInDialog(bool isSkippingDialog)
        {
            PlayerState stateAfterFrame = PlayerState.Dialog;

            bool isDialogClosed = m_DialogSystem.UpdateDialog(isSkippingDialog);
            if(isDialogClosed)
            {
                stateAfterFrame = PlayerState.World;
            }

            return stateAfterFrame;
        }
    }
}