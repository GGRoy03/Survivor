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
        // TODO:
        // The bullets are spawning in the ground.
        //

        public override void OnUpdate(Birb birb, BirbBehavior behavior, PlayerController player, EnemyAnimator animator, AudioSystem audio)
        {  
            if(m_TransitionHandle == null)
            {
                var birbBullet = birb.AcquireBullet();
                if(birbBullet != null)
                {
                    birbBullet.OnSpawn(
                        direction: Math.DirectionTowards(transform.position, player.transform.position),
                        position:  transform.position,
                        speed:     behavior.BulletSpeed,
                        damage:    behavior.BulletDamage
                        );
                }

                m_TransitionHandle = StartCoroutine(TransitionToIdle(birb));    
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