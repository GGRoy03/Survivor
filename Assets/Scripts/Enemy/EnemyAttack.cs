using UnityEngine;

using Survivor.Player;

namespace Survivor.Enemy
{
    public class EnemyAttack : EnemyState
    {
        private float m_TimeOnLastAttack;

        public EnemyAttack()
        {
            m_TimeOnLastAttack = Time.time;
        }

        public override AnimationInfo OnUpdate(Behavior behavior, PlayerController player, EnemyController controller)
        {
            float currentTime         = Time.time;
            float timeSinceLastAttack = currentTime - m_TimeOnLastAttack;
            bool  isAttacking         = timeSinceLastAttack >= behavior.AttackSpeed;

            if (isAttacking)
            {
                m_TimeOnLastAttack = currentTime;
                controller.ChangeState(controller.EnemyChaseState);
            }
            else
            {
                //
                // TODO:
                // This is a bit wonky.
                //

                controller.ChangeState(controller.EnemyChaseState);
            }

            var result = new AnimationInfo()
            {
                Speed       = 0.0f,
                Attacked    = isAttacking,
                Died        = false,
                IsDefending = false,
            };
            return result;
        }

    }
}