using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private float m_PlayerMoveSpeed;

    private Camera m_Camera;

    private void Start()
    {
        m_Camera = Camera.main;
    }

    public Vector3 MoveOnGround(Vector2 moveInput)
    {
        Vector3 cameraForward = new(m_Camera.transform.forward.x, 0.0f, m_Camera.transform.forward.z);
        Vector3 cameraRight   = new(m_Camera.transform.right.x, 0.0f  , m_Camera.transform.right.z);
        Vector3 moveDirection = moveInput.x * cameraRight + moveInput.y * cameraForward;
        float   moveSpeed     = m_PlayerMoveSpeed * Time.deltaTime;
        Vector3 moveAmount    = moveSpeed * moveDirection;

        return moveAmount;
    }
}
