using UnityEngine;

using Survivor.Player;

namespace Survivor.Enemy
{
    public class EnemyDead : EnemyState
    {
        public override void OnUpdate(Behavior behavior, EnemyAnimator animator, PlayerController player, EnemyController controller)
        {
            Debug.Log("I should die!");
        }
    }
}