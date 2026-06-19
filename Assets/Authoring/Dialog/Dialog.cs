using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "Scriptable Objects/Dialog")]
public class Dialog : ScriptableObject
{
    [System.Serializable]
    public struct CharacterSlice
    {
        public Color32 TextColor;
        public string  Content;
        public float   TransitionTime;
    }

    [System.Serializable]
    public struct DialogString
    {
        public CharacterSlice[] Slices;
        public Color            BackgroundColor;
    }

    public DialogString[] m_Strings;
}