using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas m_Canvas;

    [Header("Behavior")]
    [SerializeField]            private Transform m_PlayerTransform;
    [SerializeField, Min(0.0f)] private float     m_SqrTriggerRadius;

    private void Start()
    {
        if (!m_PlayerTransform)
        {
            m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if(m_PlayerTransform)
        {
            Vector2 playerPosition = new(m_PlayerTransform.position.x , m_PlayerTransform.position.z);
            Vector2 canvasPosition = new(m_Canvas.transform.position.x, m_Canvas.transform.position.z);
            Vector2 canvasToPlayer = playerPosition - canvasPosition;
            float   sqrDistance    = canvasToPlayer.sqrMagnitude;

            if(sqrDistance <= m_SqrTriggerRadius)
            {
                m_Canvas.gameObject.SetActive(true);
            }
            else
            {
                m_Canvas.gameObject.SetActive(false);
            }

            Debug.Log(sqrDistance);
        }
    }
}
