using Survivor.Audio;
using Survivor.Player;

using UnityEngine;

namespace Survivor.Enemy
{
    public class CactoroDefend : CactoroState
    {
        private float m_TimeElapsed;
        private float m_Duration;

        public override void OnUpdate(Cactoro cactoro, CactoroBehavior behavior, EnemyAnimator animator, PlayerController player, AudioSystem audio)
        {
            if(m_TimeElapsed == 0.0f)
            {
                m_Duration = Random.Range(behavior.MinimumDefendTime, behavior.MaximumDefendTime);
            }

            if (m_TimeElapsed >= m_Duration)
            {
                m_TimeElapsed = 0.0f;

                cactoro.ChangeState(cactoro.Idle);
            }
            else
            {
                m_TimeElapsed += Time.deltaTime;
            }

            Debug.Log("Defend!");
        }

        public override void OnAttacked(Cactoro cactoro)
        {
        }
    }
}
