using UnityEngine;

using Survivor.Core;

namespace Survivor.Enemy
{
    public class EnemyAnimator : StateAnimator
    {
        public static int IsWalking   { get; } = Animator.StringToHash("IsWalking");
        public static int IsDefending { get; } = Animator.StringToHash("IsDefending");
        public static int Attacked    { get; } = Animator.StringToHash("Attacked");
        public static int Died        { get; } = Animator.StringToHash("Died");
    }
}