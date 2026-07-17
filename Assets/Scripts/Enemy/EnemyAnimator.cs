using UnityEngine;

using Survivor.Core;


namespace Survivor.Enemy
{
    public struct AnimationInfo
    {
        public float Speed;
        public bool  Died;
        public bool  Attacked;
        public bool  IsDefending;
    }

    public class EnemyAnimator : StateAnimator<AnimationInfo>
    {
        [Header("Transitions")]
        [SerializeField] private float m_WalkToRunThresold;

        private static readonly int Idle   = Animator.StringToHash("Idle");
        private static readonly int Walk   = Animator.StringToHash("Walk");
        private static readonly int Run    = Animator.StringToHash("Run");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Death  = Animator.StringToHash("Death");

        protected override int GetAnimationState(AnimationInfo info)
        {
            if (info.Died)                         return Death;
            if (info.Attacked)                     return Attack;
            if (info.Speed >= m_WalkToRunThresold) return Run;
            if (info.Speed > 0.0f)                 return Walk;

            return Idle;
        }
    }
}