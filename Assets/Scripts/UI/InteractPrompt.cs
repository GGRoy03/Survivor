using UnityEngine;

public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private float m_SqrInteractDistance;
    [SerializeField] private Canvas m_Canvas;

    private Transform  m_PlayerTransform;
    private PlayerCore m_PlayerCore;

    void Start()
    {
        var playerObject = GameObject.FindWithTag("Player");
        m_PlayerTransform = playerObject.transform;
        m_PlayerCore      = playerObject.GetComponent<PlayerCore>();
    }

    void Update()
    {
        var playerState = m_PlayerCore.State;
        if(playerState == PlayerCore.GameState.World)
        {
            Vector3 playerPosition    = m_PlayerTransform.position;
            Vector3 promptPosition    = transform.position;
            float   promptSqrDistance = (promptPosition - playerPosition).sqrMagnitude;

            if (promptSqrDistance < m_SqrInteractDistance)
            {
                m_Canvas.gameObject.SetActive(true);
            }
            else
            {
                m_Canvas.gameObject.SetActive(false);
            }
        }
        else
        {
            m_Canvas.gameObject.SetActive(false);
        }
    }   
}
