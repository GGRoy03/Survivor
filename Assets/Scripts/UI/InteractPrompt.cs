using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [Header("Animation")]
    [SerializeField] private float m_Duration;
    private float m_Elapsed;

    private bool      m_IsVisible;
    private Coroutine m_AnimationHandle;

    public void SetVisibility(bool value)
    {
        if(value != m_IsVisible)
        {
            float targetAlpha = value ? 1.0f : 0.0f;

            if (m_AnimationHandle != null)
            {
                StopCoroutine(m_AnimationHandle);
            }

            m_Elapsed         = 0.0f;
            m_AnimationHandle = StartCoroutine(AnimatePrompt(targetAlpha));
            m_IsVisible       = value;
        }
    }

    private IEnumerator AnimatePrompt(float targetAlpha)
    {
        float startAlpha = m_CanvasGroup.alpha;
        while(m_Elapsed < m_Duration)
        {     
            float progress = m_Elapsed / m_Duration;

            m_CanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            m_Elapsed          += Time.deltaTime;

            yield return null;
        }

        m_CanvasGroup.alpha = targetAlpha;
        m_AnimationHandle   = null;
    }
}