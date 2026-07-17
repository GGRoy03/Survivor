using Survivor.Core;
using UnityEngine;

namespace Survivor.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : StateAnimator<AnimationInfo>
    {
        private static readonly int Idle   = Animator.StringToHash("Idle");
        private static readonly int Walk   = Animator.StringToHash("Walk");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Death  = Animator.StringToHash("Death");

        protected override int GetAnimationState(AnimationInfo info)
        {
            return Idle;
        }
    }

    public struct AnimationInfo
    {
        
    }
}
