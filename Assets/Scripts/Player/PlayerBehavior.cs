using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerBehavior")]
public class PlayerBehavior : ScriptableObject
{
    [field:SerializeField] public float AttackSpeed         { get; private set;}
    [field:SerializeField] public float AttackCost          { get; private set;}
    [field:SerializeField] public float HungerDecreaseRate  { get; private set;}
    [field:SerializeField] public float StaminaIncreaseRate { get; private set;}
    [field:SerializeField] public float MoveSpeed           { get; private set;}
}
