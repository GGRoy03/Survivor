using UnityEngine;

namespace Survivor.Enemy
{
    [CreateAssetMenu(menuName = "Enemy/CactoroBehavior")]
    public class CactoroBehavior : ScriptableObject
    {
        [field:SerializeField] public float ChaseRange { get; private set;}
        [field:SerializeField] public int   ChanceToEnterDefendState { get; private set;}
        [field:SerializeField] public float AttackRange { get; private set;}
        [field:SerializeField] public float MoveSpeed { get; private set;}
        [field:SerializeField] public float BodyRotationSpeed{ get; private set;}
        [field:SerializeField] public float MinimumDefendTime { get; private set;}
        [field:SerializeField] public float MaximumDefendTime { get; private set;}
        [field:SerializeField] public float AttackLockTime { get; private set;}
    }
}