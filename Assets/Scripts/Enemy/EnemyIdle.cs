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

        public override void OnUpdate(Behavior behavior, EnemyAnimator animator, PlayerController player, EnemyController controller)
        {
            float sqrDistanceBetween = Math.SqrDistanceBetweenTransform(m_PlayerTransform, m_EnemyTransform);
            float sqrChaseRange      = behavior.ChaseRange * behavior.ChaseRange;

            if(sqrDistanceBetween <= sqrChaseRange)
            {
                controller.ChangeState(controller.EnemyChaseState);
            }
        }
    }
}
