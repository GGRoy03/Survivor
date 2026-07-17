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

        public override AnimationInfo OnUpdate(Behavior behavior, PlayerController player, EnemyController controller)
        {
            float sqrDistanceBetween = Math.SqrDistanceBetweenTransform(m_PlayerTransform, m_EnemyTransform);
            float sqrAttackRange     = behavior.ChaseRange * behavior.ChaseRange;
            if(sqrDistanceBetween <= sqrAttackRange)
            {
                controller.ChangeState(controller.EnemyAttackState);
            }
            else
            {
                //
                // TODO:
                // Maybe toggle the defend state or something. RNG based or whatever.
                //

                int randomValue = Random.Range(0, behavior.ChanceToEnterDefendState);
                if(randomValue == 0)
                {
                    controller.ChangeState(controller.EnemyDefendState);
                }
                else
                {
                    Vector3 enemyToPlayer = Math.DirectionTowards(m_EnemyTransform.position, m_PlayerTransform.position);
                    if (enemyToPlayer.sqrMagnitude > Mathf.Epsilon)
                    {
                        //
                        // Make the enemy look towards the player.
                        //

                        float singleStep = 2.0f * Time.deltaTime;
                        Vector3 lookDirection = Vector3.RotateTowards(m_EnemyTransform.forward, enemyToPlayer, singleStep, 0.0f);
                        m_EnemyTransform.rotation = Quaternion.LookRotation(lookDirection);

                        //
                        // Move the enemy in the player's direction.
                        //

                        float moveSpeed = behavior.MoveSpeed * Time.deltaTime;
                        m_EnemyTransform.Translate(moveSpeed * Vector3.forward);
                    }
                }
            }

            var result = new AnimationInfo()
            {
                Speed    = 0.0f,
                Attacked = false,
                Died     = false,
            };
            return result;
        }
    }
}
