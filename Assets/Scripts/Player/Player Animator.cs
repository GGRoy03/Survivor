using Survivor.Core;
using UnityEngine;

namespace Survivor.Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : StateAnimator
    {
        public static int IsWalking { get; } = Animator.StringToHash("IsWalking");
        public static int Attacked  { get; } = Animator.StringToHash("Attacked");
        public static int Died      { get; } = Animator.StringToHash("Died");
    }
}