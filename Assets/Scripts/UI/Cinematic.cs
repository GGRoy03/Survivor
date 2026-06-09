using UnityEngine;
using UnityEngine.UI;

public class CameraCore : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Image      m_TopCinematicBar;
    [SerializeField] private Image      m_BotCinematicBar;
                     private PlayerCore m_PlayerCore;
                     private Camera     m_Camera;

    [Header("Cinematic")]
    [SerializeField] private float m_CinematicTransitionTime;
                     private float m_CinematicTransitionProgress;

    [SerializeField] private float m_OrthographicSizeInCinematic;
                     private float m_OrthographicSizeNotInCinematic;

    [SerializeField] private float m_BarSizeInCinematic;
                     private float m_TopBarSizeNotInCinematic;
                     private float m_BotBarSizeNotInCinematic;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] bool m_IsInDialog;
#endif

    void Start()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        m_PlayerCore = playerObject.GetComponent<PlayerCore>();

        m_Camera                         = Camera.main;
        m_OrthographicSizeNotInCinematic = m_Camera.orthographicSize;
    }

    public static float EaseInOutCubic(float t)
    {
        return t < 0.5f
            ? 4.0f * t * t * t
            : 1.0f - Mathf.Pow(-2.0f * t + 2.0f, 3.0f) / 2.0f;
    }

    void Update()
    {
        m_CinematicTransitionProgress += m_IsInDialog ? Time.deltaTime : -Time.deltaTime;
        m_CinematicTransitionProgress = Mathf.Clamp(m_CinematicTransitionProgress, 0.0f, m_CinematicTransitionTime);

        float cinematicProgress     = m_CinematicTransitionProgress / m_CinematicTransitionTime;
        float easedCinematicProgress = EaseInOutCubic(cinematicProgress);

        UpdateCinematicBarSize(m_TopCinematicBar, m_TopBarSizeNotInCinematic, easedCinematicProgress);
        UpdateCinematicBarSize(m_BotCinematicBar, m_BotBarSizeNotInCinematic, easedCinematicProgress);

        m_Camera.orthographicSize = Mathf.Lerp(m_OrthographicSizeNotInCinematic, m_OrthographicSizeInCinematic, easedCinematicProgress);
    }

    private void UpdateCinematicBarSize(Image image, float sizeNotInCinematic, float progress)
    {
        var   topBarRect = image.GetComponent<RectTransform>();
        float topBarSize = Mathf.Lerp(sizeNotInCinematic, m_BarSizeInCinematic, progress);
        topBarRect.sizeDelta = new(topBarRect.sizeDelta.x, topBarSize);
    }
}
