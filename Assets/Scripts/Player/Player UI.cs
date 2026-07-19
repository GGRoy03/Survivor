using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Survivor.Event;

namespace Survivor.Player
{
    public class PlayerHUD : MonoBehaviour
    {    
        [Header("Stat Bars")]
        [SerializeField] private StatBar m_HealthBar;
        [SerializeField] private StatBar m_StaminaBar;
        [SerializeField] private StatBar m_HungerBar;

        [Header("Animations")]
        [SerializeField] private float m_ShakeDuration = 0.25f;
        [SerializeField] private float m_ShakeStrength = 8.0f;

        private Coroutine m_ShakeBarHandle;

        //
        // Unity Hooks
        //

        private void OnEnable()
        {
            EventManager.Instance.AddListener<EventPlayerStatChanged>(OnPlayerStateChanged);
            EventManager.Instance.AddListener<EventPlayerAttackWithoutStamina>(OnPlayerAttackWithoutStamina);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener<EventPlayerStatChanged>(OnPlayerStateChanged);
            EventManager.Instance.RemoveListener<EventPlayerAttackWithoutStamina>(OnPlayerAttackWithoutStamina);
        }

        //
        // Event Handlers
        //

        public void OnPlayerStateChanged(EventPlayerStatChanged payload)
        {
            var stat = payload.Stat;
            switch (stat.Type)
            {
                case PlayerStatType.Health:  m_HealthBar.SetImageWidth(stat); break;
                case PlayerStatType.Hunger:  m_HungerBar.SetImageWidth(stat); break;
                case PlayerStatType.Stamina: m_StaminaBar.SetImageWidth(stat); break;
            }
        }

        public void OnPlayerAttackWithoutStamina(EventPlayerAttackWithoutStamina payload)
        {
            if (m_ShakeBarHandle == null)
            {
                m_ShakeBarHandle = StartCoroutine(ShakeStatBar(m_StaminaBar, m_ShakeDuration, m_ShakeStrength));
            }
        }

        //
        // Internal Helpers
        //

        [System.Serializable]
        private struct StatBar
        {
            [SerializeField] private RectTransform m_BarContainer;
            [SerializeField] private RectTransform m_Bar;

            public readonly RectTransform Container => m_BarContainer;
            public readonly RectTransform Bar       => m_Bar;

            public readonly void SetImageWidth(PlayerStat stat)
            {
                if(m_BarContainer != null && m_Bar != null)
                {
                    float maxWidth   = m_BarContainer.sizeDelta.x;
                    float valueRatio = Mathf.Clamp(stat.Current / stat.Maximum, 0.0f, 1.0f);
                    float imageWidth = maxWidth * valueRatio;

                    m_Bar.sizeDelta = new(imageWidth, m_BarContainer.sizeDelta.y);
                }
            }
        }

        private IEnumerator ShakeStatBar(StatBar statBar, float duration, float strength)
        {
            var     containerTransform = statBar.Container;
            Vector2 originalPos        = containerTransform.anchoredPosition;

            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                float offsetX = Random.Range(-1f, 1f) * strength;
                float offsetY = Random.Range(-1f, 1f) * strength;
                containerTransform.anchoredPosition = originalPos + new Vector2(offsetX, offsetY);

                elapsed += Time.deltaTime;

                yield return null;
            }

            containerTransform.anchoredPosition = originalPos;

            m_ShakeBarHandle = null;
        }
    }

    public struct EventPlayerStatChanged
    {
        public PlayerStat Stat;
    }
    
    public struct EventPlayerAttackWithoutStamina
    {

    }
}