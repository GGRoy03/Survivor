using Survivor.Audio;
using Survivor.Player;

namespace Survivor.Enemy
{
    public class CactoroDead : CactoroState
    {
        private bool m_DeathTriggered;

        public override void OnUpdate(Cactoro cactoro, CactoroBehavior behavior, EnemyAnimator animator, PlayerController player, AudioSystem audio)
        {
            if (!m_DeathTriggered)
            {
                animator.SetParam(EnemyAnimator.Died);

                m_DeathTriggered = true;
            }
            else if(!animator.IsClipPlaying())
            {
                Destroy(gameObject);
            }
        }

        public override void OnAttacked(Cactoro cactoro)
        {
            // No-Op
        }
    }
}
