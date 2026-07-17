using UnityEngine;

using Survivor.Player;

namespace Survivor.Enemy
{
    public class EnemyDead : EnemyState
    {
        public override AnimationInfo OnUpdate(Behavior behavior, PlayerController player, EnemyController controller)
        {
            Debug.Log("I should die!");

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