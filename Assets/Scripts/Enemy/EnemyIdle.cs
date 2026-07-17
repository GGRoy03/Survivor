using UnityEngine;

using Survivor.Player;

namespace Survivor.Enemy
{
    public class EnemyIdle : EnemyState
    {
        private readonly Transform m_PlayerTransform;
        private readonly Transform m_EnemyTransform;

        public EnemyIdle(PlayerController player, EnemyController controller)
        {
            m_PlayerTransform = player.transform;
            m_EnemyTransform  = controller.transform;
        }

        public override AnimationInfo OnUpdate(Behavior behavior, PlayerController player, EnemyController controller)
        {
            float sqrDistanceBetween = Math.SqrDistanceBetweenTransform(m_PlayerTransform, m_EnemyTransform);
            float sqrChaseRange      = behavior.ChaseRange * behavior.ChaseRange;
            if(sqrDistanceBetween <= sqrChaseRange)
            {
                controller.ChangeState(controller.EnemyChaseState);
            }

            var result = new AnimationInfo()
            {
                Speed       = 0.0f,
                Attacked    = false,
                Died        = false,
                IsDefending = false,
            };
            return result;
        }
    }
}
