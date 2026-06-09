using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    private struct StatBar
    {
        public Image Image;
        public float MaxWidth;

        public StatBar(Image image)
        {
            Image    = image;
            MaxWidth = image.GetComponent<RectTransform>().sizeDelta.x;
        }
    }
        
    [Header("Dependencies")]
    [SerializeField] private Image m_HealthBarImage;
    [SerializeField] private Image m_StaminaBarImage;
    [SerializeField] private Image m_HungerBarImage;

    private StatBar    m_HealthBar;
    private StatBar    m_StaminaBar;
    private StatBar    m_HungerBar;
    private PlayerCore m_PlayerCore;

    void Start()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        m_PlayerCore = playerObject.GetComponent<PlayerCore>();

        m_HealthBar  = new(m_HealthBarImage);
        m_StaminaBar = new(m_StaminaBarImage);
        m_HungerBar  = new(m_HungerBarImage);
    }

    void Update()
    {
        SetImageWidth(m_PlayerCore.Health  / m_PlayerCore.MaxHealth , m_HealthBar);
        SetImageWidth(m_PlayerCore.Stamina / m_PlayerCore.MaxStamina, m_StaminaBar);
        SetImageWidth(m_PlayerCore.Hunger  / m_PlayerCore.MaxHunger , m_HungerBar);
    }

    private void SetImageWidth(float currentValueRatio, StatBar statBar)
    {
        float clampedRatio = Mathf.Clamp(currentValueRatio, 0.0f, 1.0f);
        float imageWidth   = statBar.MaxWidth * clampedRatio;

        var rectTransform = statBar.Image.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new(imageWidth, rectTransform.sizeDelta.y);
    }
}
