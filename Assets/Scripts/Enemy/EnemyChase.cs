using Survivor.Player;

using UnityEngine;

namespace Survivor.Enemy
{
    public class EnemyChase : EnemyState
    {
        private readonly Transform m_PlayerTransform;
        private readonly Transform m_EnemyTransform;

        public EnemyChase(PlayerController player, EnemyController controller)
        {
            m_PlayerTransform = player.transform;
            m_EnemyTransform  = controller.transform;
        }

        public override void OnUpdate(Behavior behavior, EnemyAnimator animator, PlayerController player, EnemyController controller)
        {
            float sqrDistanceBetween = Math.SqrDistanceBetweenTransform(m_PlayerTransform, m_EnemyTransform);
            float sqrAttackRange     = behavior.AttackRange * behavior.AttackRange;
            float sqrChaseRange      = behavior.ChaseRange  * behavior.ChaseRange;

            if(sqrDistanceBetween <= sqrAttackRange)
            {
                controller.ChangeState(controller.EnemyAttackState);

                animator.SetParam(false, EnemyAnimator.IsWalking);
            }
            else if(sqrDistanceBetween > sqrChaseRange)
            {
                controller.ChangeState(controller.EnemyIdleState);

                animator.SetParam(false, EnemyAnimator.IsWalking);
            }
            else
            {
                int randomValue = Random.Range(0, behavior.ChanceToEnterDefendState);
                if(randomValue == 0)
                {
                    controller.ChangeState(controller.EnemyDefendState);
                }
                else
                {
                    //
                    // Make the enemy look towards the player.
                    //

                    Vector3 enemyToPlayer = Math.DirectionTowards(m_EnemyTransform.position, m_PlayerTransform.position);
                    m_EnemyTransform.rotation = Math.LookTowards(m_EnemyTransform.forward, enemyToPlayer, 2.0f);

                    //
                    // Move the enemy in the player's direction.
                    //

                    float moveSpeed = behavior.MoveSpeed * Time.deltaTime;
                    m_EnemyTransform.Translate(moveSpeed * Vector3.forward);

                    animator.SetParam(true, EnemyAnimator.IsWalking);
                }
            }
        }
    }
}
