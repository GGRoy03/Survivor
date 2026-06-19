using UnityEngine;
using UnityEngine.UI;

using Survivor.Player;

public class PlayerHUD : MonoBehaviour
{    
    [Header("Dependencies")]
    [SerializeField] private StatBar m_HealthBar;
    [SerializeField] private StatBar m_StaminaBar;
    [SerializeField] private StatBar m_HungerBar;

    private PlayerSystem m_PlayerCore;

    void Start()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        m_PlayerCore = playerObject.GetComponent<PlayerSystem>();
    }

    void Update()
    {
        m_HealthBar.SetImageWidth(m_PlayerCore.Health   / m_PlayerCore.MaxHealth);
        m_StaminaBar.SetImageWidth(m_PlayerCore.Stamina / m_PlayerCore.MaxStamina);
        m_HungerBar.SetImageWidth(m_PlayerCore.Hunger   / m_PlayerCore.MaxHunger);;
    }

    [System.Serializable]
    private struct StatBar
    {
        [SerializeField] private Image Image;
                         private float MaxWidth;

        public void SetImageWidth(float valueRatio)
        {
            if(Image != null)
            {
                var rectTransform = Image.GetComponent<RectTransform>();

                if (MaxWidth == 0.0f)
                {
                    MaxWidth = rectTransform.sizeDelta.x;
                }

                float clampedRatio = Mathf.Clamp(valueRatio, 0.0f, 1.0f);
                float imageWidth   = MaxWidth * clampedRatio;
                rectTransform.sizeDelta = new(imageWidth, rectTransform.sizeDelta.y);
            }
        }
    }
}
