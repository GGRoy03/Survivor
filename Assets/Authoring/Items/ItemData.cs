using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
[System.Serializable]
public class ItemData : ScriptableObject
{
    public string Name;
    public int    HealthDelta;
    public int    StaminaDelta;
    public int    HungerDelta;

    public Sprite Icon;
}
