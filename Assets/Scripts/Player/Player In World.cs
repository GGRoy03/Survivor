using UnityEngine;
using Survivor.Event;

namespace Survivor.Player
{
    public class PlayerInWorld : PlayerState
    {
        //
        // NOTE:
        // If the camera ever rotates, we should fallback to caching the camera and not the vectors.
        //
    
        private Vector3   m_CameraForward;
        private Vector3   m_CameraRight;
        private Transform m_Transform;

        //
        // State-Machine Hooks
        //
    
        public override void OnEnter(PlayerController Controller)
        {
            m_CameraForward = new(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
            m_CameraRight   = new(Camera.main.transform.right.x  , 0.0f, Camera.main.transform.right.z);
            m_Transform     = Controller.transform;
        }
    
        public override void OnUpdate(PlayerInputProvider inputs, PlayerBehavior behavior, DialogSystem dialogSystem, PlayerController controller)
        {
            Vector3 playerPosition      = m_Transform.position;
            float   playerInteractRange = 0.0f;
    
            if(inputs.IsAttacking)
            {
                float staminaCost = behavior.AttackStaminaCost;
                if(controller.Stamina.Current >= staminaCost)
                {
                    controller.Stamina -= staminaCost;
                }
                else
                {
                    EventManager.Instance.PushEvent(new EventPlayerAttackWithoutStamina());
                }
            }
            else if(inputs.IsInteracting)
            {
                Dialog bestDialog = TryFindBestDialog(playerPosition, playerInteractRange);
                if (bestDialog)
                {
                    if(dialogSystem.TryEnterDialog(bestDialog))
                    {
                        controller.ChangeState(new PlayerInDialog());
                    }
                }
            }
            else if(inputs.IsTogglingInventory)
            {
                controller.ChangeState(new PlayerInInventory());
            }
            else
            {
                Vector2 moveInput     = inputs.MoveInput;
                Vector3 moveDirection = moveInput.x * m_CameraRight + moveInput.y * m_CameraForward;
                float   moveSpeed     = behavior.MoveSpeed * Time.deltaTime;
                Vector3 moveAmount    = moveSpeed * moveDirection;

                m_Transform.Translate(moveAmount);
            }

            //
            // NOTE:
            // We could implement these as a coroutine, but I don't see why. We probably do not want
            // stats to be modified as the player is not in the active state (which is the world state)
            // and adding coroutines here would just make the code more complicated as far as I can tell.
            // This might lead to duplicate code if there are more than one state that could tick these values
            // (maybe a combat state?), but I wager that in those cases the increment/decrement would be different
            // anyway...
            //

            {
                controller.Stamina += behavior.StaminaIncreaseRate;
                controller.Hunger  -= behavior.HungerDecreaseRate;

                EventManager.Instance.PushEvent(new EventPlayerStatChanged()
                {
                    Stat = controller.Stamina
                });

                EventManager.Instance.PushEvent(new EventPlayerStatChanged()
                {
                    Stat = controller.Hunger
                });
            }

            SetInteractPromptVisiblity(playerPosition, playerInteractRange);
        }

        public override AnimationInfo OnAnimate()
        {
            return new AnimationInfo();
        }
        
        //
        // Internal Helpers
        //

        private Dialog TryFindBestDialog(Vector3 playerPosition, float range)
        {
            var interactLayer     = LayerMask.GetMask("NPC");
            var interactColliders = Physics.OverlapSphere(playerPosition, range, interactLayer);
    
            Dialog bestDialog     = null;
            float  closestSqrDist = float.MaxValue;
            foreach (var collider in interactColliders)
            {
                var colliderObject = collider.gameObject;
                if (colliderObject.TryGetComponent(out DialogItem dialogItem))
                {
                    float sqrDistanceToPlayer = Vector3.SqrMagnitude(playerPosition - collider.transform.position);
                    if (sqrDistanceToPlayer < closestSqrDist)
                    {
                        closestSqrDist = sqrDistanceToPlayer;
                        bestDialog     = dialogItem.Dialog;
                    }
                }      
            }
    
            return bestDialog;
        }

        //
        // NOTE:
        // Could we not.. uhm, write some form of collider iterator?
        // Probably overkill, but there's a lot of boilerplate we have to deal with simply to iterate
        // colliders from a query..
        //

        private void SetInteractPromptVisiblity(Vector3 playerPosition, float range)
        {
            var interactLayer     = LayerMask.GetMask("Prompt");
            var interactColliders = Physics.OverlapSphere(playerPosition, range * 1.5f, interactLayer);
            var sqrRange          = range * range;
    
            foreach (var collider in interactColliders)
            {
                var colliderObject = collider.gameObject;
                if (colliderObject.TryGetComponent<InteractPrompt>(out var prompt))
                {
                    var colliderPosition    = collider.transform.position;
                    var colliderSqrDistance = Vector3.SqrMagnitude(colliderPosition - playerPosition);
    
                    bool isInRange = colliderSqrDistance <= sqrRange;
                    prompt.SetVisibility(isInRange);
                }
            }
        }
    }
}