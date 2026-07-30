using Survivor.Audio;
using Survivor.Player;

using System.Collections;

using UnityEngine;

namespace Survivor.Enemy
{
    public class BirbAttack : BirbState
    {
        //
        // Interface Implementation
        //

        public override void OnUpdate(Birb birb, BirbBehavior behavior, PlayerController player, EnemyAnimator animator, AudioSystem audio)
        {  
            if(m_TransitionHandle == null)
            {
                var birbBullet = birb.AcquireBullet();
                if(birbBullet != null)
                {
                    birbBullet.OnSpawn(
                        direction: Math.Direction(transform.position, player.transform.position),
                        position:  birb.BulletSpawnPoint,
                        speed:     behavior.BulletSpeed,
                        damage:    behavior.BulletDamage
                        );
                }

                m_TransitionHandle = StartCoroutine(TransitionToIdle(birb));

                animator.SetParam(EnemyAnimator.Attacked);
            }
        }

        //
        // Transition
        //

        private static readonly WaitForSeconds m_WaitTimer = new(2.0f);

        private Coroutine m_TransitionHandle;

        private IEnumerator TransitionToIdle(Birb birb)
        {
            yield return m_WaitTimer;

            m_TransitionHandle = null;

            birb.ChangeState(birb.Idle);
        }
    }
}