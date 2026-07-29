using UnityEngine;

namespace Survivor.Enemy
{
    [CreateAssetMenu(menuName = "Enemy/BirbBehavior")]
    public class BirbBehavior : ScriptableObject
    {
        [field:SerializeField] public float SightAngle { get; private set;}
        [field:SerializeField] public float RotationSpeed { get; private set;}
        [field:SerializeField] public float BulletDamage { get; private set;}
        [field:SerializeField] public float BulletSpeed { get; private set;}
        [field:SerializeField] public float BulletRange { get; private set;}

    }
}