using Survivor.Audio;
using Survivor.Player;
using UnityEngine;

namespace Survivor.Enemy
{
    public class CactoroIdle : CactoroState
    {
        public override void OnUpdate(Cactoro cactoro, CactoroBehavior behavior, EnemyAnimator animator, PlayerController player, AudioSystem audio)
        {
            float sqrDistanceBetween = Math.SqrDistance(player.transform, cactoro.transform);
            float sqrChaseRange      = behavior.ChaseRange * behavior.ChaseRange;

            if(sqrDistanceBetween <= sqrChaseRange)
            {
                cactoro.ChangeState(cactoro.Chase);
            }
        }

        public override void OnAttacked(Cactoro cactoro)
        {
            cactoro.ChangeState(cactoro.Dead);
        }
    }
}
