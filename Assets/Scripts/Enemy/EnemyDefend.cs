using UnityEngine;

using Survivor.Player;

namespace Survivor.Enemy
{
    public class EnemyDefend : EnemyState
    {
        public override void OnUpdate(Behavior behavior, EnemyAnimator animator, PlayerController player, EnemyController controller)
        {
            int  value       = Random.Range(0, behavior.ChanceToLeaveDefendState);
            bool isDefending = value != 0;

            if (!isDefending)
            {
                controller.ChangeState(controller.EnemyIdleState);
            }

            animator.SetParam(isDefending, EnemyAnimator.IsDefending);
        }
    }
}