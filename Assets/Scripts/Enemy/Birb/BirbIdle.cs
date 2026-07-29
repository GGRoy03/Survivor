using Survivor.Audio;
using Survivor.Player;
using UnityEngine;

namespace Survivor.Enemy
{
    public class BirbIdle : BirbState
    {
        //
        // Interface Implementation
        //

        private Transform m_PlayerTransform;
        public override void OnUpdate(Birb birb, BirbBehavior behavior, PlayerController player, EnemyAnimator animator, AudioSystem audio)
        {
            if(m_PlayerTransform == null)
            {
                m_PlayerTransform = player.transform;
            }

            //
            // NOTE:
            // We always check the sight angle first, because otherwise as we are
            // changing from the attack state to this state, the rotation would be
            // done once for no reasons.
            //

            Vector3 enemyPosition  = transform.position;
            Vector3 playerPosition = m_PlayerTransform.position;
            Vector3 enemyToPlayer  = Vector3.Normalize(playerPosition - enemyPosition);
            float   angle          = Vector3.Angle(transform.forward, enemyToPlayer);

            if(angle <= 10.0f)
            {
                birb.ChangeState(birb.Attack);
            }
            else
            {
                transform.Rotate(Vector3.up, behavior.RotationSpeed * Time.deltaTime);
            }
        }
    }
}
