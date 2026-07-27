using System.Collections;

using UnityEngine;

namespace Survivor.Player
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerController m_PlayerController;
        [SerializeField] private RectTransform    m_PlayerUIContainer;

        [Header("Stat Bars")]
        [SerializeField] private StatBar m_HealthBar;
        [SerializeField] private StatBar m_StaminaBar;
        [SerializeField] private StatBar m_HungerBar;

        [Header("Animations")]
        [SerializeField] private float m_ShakeDuration = 0.25f;
        [SerializeField] private float m_ShakeStrength = 8.0f;

        //
        // Unity Hooks
        //

        private void Update()
        {
            if(GameController.Current == GameController.GameMode.Gameplay)
            {
                m_PlayerUIContainer.gameObject.SetActive(true);

                //
                // Update the stat bars.
                //

                m_HealthBar.SetImageWidth(m_PlayerController.Health);
                m_HungerBar.SetImageWidth(m_PlayerController.Hunger);
                m_StaminaBar.SetImageWidth(m_PlayerController.Stamina);

                //
                //
                //

                if(m_PlayerController.AttackedWithoutStamnia)
                {
                    if (m_ShakeBarHandle == null)
                    {
                        m_ShakeBarHandle = StartCoroutine(ShakeStatBar(m_StaminaBar, m_ShakeDuration, m_ShakeStrength));
                    }
                }
            }
            else
            {
                m_PlayerUIContainer.gameObject.SetActive(false);
            }
        }

        //
        // Animations
        //

        private Coroutine m_ShakeBarHandle;

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
    }
}