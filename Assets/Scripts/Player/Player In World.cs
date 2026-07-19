using UnityEngine;
using Survivor.Event;

namespace Survivor.Player
{
    public class PlayerInWorld : PlayerState
    {
        private Transform m_Transform;
        private Camera    m_Camera;

        //
        // State-Machine Hooks
        //
    
        public override void OnEnter(PlayerController Controller)
        {
            m_Camera    = Camera.main;
            m_Transform = Controller.transform;
        }
    
        public override void OnUpdate(PlayerInputProvider inputs, PlayerAnimator animator, PlayerBehavior behavior, DialogSystem dialogSystem, PlayerController controller)
        {
            Vector3 playerPosition      = m_Transform.position;
            float   playerInteractRange = 0.0f;
    
            if(inputs.IsAttacking)
            {
                //
                // TODO:
                // We need an attack timer :)
                //

                float staminaCost = behavior.AttackStaminaCost;
                if(controller.Stamina.Current >= staminaCost)
                {
                    controller.Stamina -= staminaCost;

                    animator.SetParam(PlayerAnimator.Attacked);
                }
                else
                {
                    EventManager.Instance.PushEvent(new EventPlayerAttackWithoutStamina());
                }
            }
            else if(inputs.IsInteracting)
            {
                Dialog bestDialog = TryFindBestDialog(playerPosition, playerInteractRange);
                if (bestDialog && dialogSystem.TryEnterDialog(bestDialog))
                {
                    controller.ChangeState(new PlayerInDialog());

                    animator.SetParam(false, PlayerAnimator.IsWalking);
                }
            }
            else if(inputs.IsTogglingInventory)
            {
                controller.ChangeState(new PlayerInInventory());

                animator.SetParam(false, PlayerAnimator.IsWalking);
            }
            else
            {
                //
                // TODO:
                // I have no clue what the expected movement is for these types of games.
                // There's a funny bug if we move while the mouse is really close to the player.
                // I don't know, this just sucks.
                //
                
                Vector2 moveInput = inputs.MoveInput;
                bool    isMoving  = moveInput.y > 0.0f;

                if(isMoving)
                {
                    Vector3 moveDirection = moveInput.y * Vector3.forward;
                    float   moveSpeed     = behavior.MoveSpeed * Time.deltaTime;
                    m_Transform.Translate(moveSpeed * moveDirection);
                }

                animator.SetParam(isMoving, PlayerAnimator.IsWalking);
            }

            //
            // Rotate the player towards where the mouse is placed.
            //

            {
                var groundPlane = new Plane(Vector3.up, playerPosition);
                var pointerRay  = m_Camera.ScreenPointToRay(inputs.PointerPosition);
                if(groundPlane.Raycast(pointerRay, out float distance))
                {
                    var hitPoint  = pointerRay.GetPoint(distance);
                    var direction = Vector3.Normalize(hitPoint - playerPosition);

                    m_Transform.rotation = Math.LookTowards(m_Transform.forward, direction, behavior.BodyRotationSpeedInRadiansPerSeconds);
                }
            }

            SetInteractPromptVisiblity(playerPosition, playerInteractRange);
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