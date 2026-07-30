using Survivor.Audio;
using Survivor.Player;

using System.Collections;

using UnityEngine;

namespace Survivor.Enemy
{
    public class CactoroAttack : CactoroState
    {
        public override void OnUpdate(Cactoro cactoro, CactoroBehavior behavior, EnemyAnimator animator, PlayerController player, AudioSystem audio)
        {
            if (m_TransitionHandle == null)
            {
                animator.SetParam(EnemyAnimator.Attacked);

                m_TransitionHandle = StartCoroutine(WaitForCompletion(cactoro, behavior));
            }
        }

        public override void OnAttacked(Cactoro cactoro)
        {
            cactoro.ChangeState(cactoro.Dead);
        }
        
        //
        // Transition
        //
        // NOTE:
        // This is a bit cheap, but it's really simple.
        //

        private WaitForSeconds m_WaitTimer;    
        private Coroutine      m_TransitionHandle;

        private IEnumerator WaitForCompletion(Cactoro cactoro, CactoroBehavior behavior)
        {
            if(m_WaitTimer == null)
            {
                m_WaitTimer = new(behavior.AttackLockTime);
            }
            yield return m_WaitTimer;

            m_TransitionHandle = null;

            cactoro.ChangeState(cactoro.Chase);
        }
    }
}
