using UnityEngine;

using Survivor.Player;

namespace Survivor.Enemy
{
    public class EnemyDefend : EnemyState
    {
        public override AnimationInfo OnUpdate(Behavior behavior, PlayerController player, EnemyController controller)
        {
            int  value          = Random.Range(0, behavior.ChanceToLeaveDefendState);
            bool isExitingState = value == 0;

            if (isExitingState)
            {
                controller.ChangeState(controller.EnemyIdleState);
            }

            var result = new AnimationInfo()
            {
                Speed       = 0.0f,
                Attacked    = false,
                Died        = false,
                IsDefending = !isExitingState,
            };
            return result;
        }
    }
}