using Survivor.Audio;
using Survivor.Player;

using UnityEngine;

namespace Survivor.Enemy
{
    public class CactoroChase : CactoroState
    {
        public override void OnUpdate(Cactoro cactoro, CactoroBehavior behavior, EnemyAnimator animator, PlayerController player, AudioSystem audio)
        {
            int randomValue = Random.Range(0, behavior.ChanceToEnterDefendState);
            if(randomValue != 0)
            {
                float sqrDistanceBetween = Math.SqrDistance(player.transform, cactoro.transform);

                if (sqrDistanceBetween <= (behavior.AttackRange * behavior.AttackRange))
                {
                    cactoro.ChangeState(cactoro.Attack);
                }
                else if(sqrDistanceBetween > (behavior.ChaseRange  * behavior.ChaseRange))
                {
                    cactoro.ChangeState(cactoro.Idle);
                }
                else
                {
                    var enemyToPlayer = Math.Direction(cactoro.transform.position, player.transform.position);
                    cactoro.transform.rotation = Math.LookAt(cactoro.transform.forward, enemyToPlayer, behavior.BodyRotationSpeed);

                    var moveSpeed = behavior.MoveSpeed * Time.deltaTime;
                    cactoro.transform.Translate(moveSpeed * Vector3.forward);
                }
            }
            else
            {
                cactoro.ChangeState(cactoro.Defend);
            }
        }

        public override void OnAttacked(Cactoro cactoro)
        {
            cactoro.ChangeState(cactoro.Dead);
        }
    }
}
