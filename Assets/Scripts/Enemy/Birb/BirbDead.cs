using Survivor.Audio;
using Survivor.Player;

namespace Survivor.Enemy
{
    public class BirbDead : BirbState
    {
        private bool m_DeathTriggered;

        public override void OnUpdate(Birb birb, BirbBehavior behavior, PlayerController player, EnemyAnimator animator, AudioSystem audio)
        {
            if(!m_DeathTriggered)
            {
                animator.SetParam(EnemyAnimator.Died);

                m_DeathTriggered = true;
            }
            else if(!animator.IsClipPlaying())
            {
                Destroy(gameObject);
            }
        }
    }
}