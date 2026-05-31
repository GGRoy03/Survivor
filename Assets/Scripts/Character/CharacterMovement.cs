using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionAsset m_InputMap;
                     private InputAction      m_MoveAction;

    [Header("Movement")]
    [SerializeField] private float m_PlayerMoveSpeed;

    private void Start()
    {
        m_MoveAction = m_InputMap.FindAction("Move");
    }

    private void Update()
    {
        Vector3 cameraForward = new(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
        Vector3 cameraRight   = new(Camera.main.transform.right.x  , 0.0f, Camera.main.transform.right.z);
        Vector2 moveInput     = m_MoveAction.ReadValue<Vector2>().normalized;
        Vector3 moveDirection = moveInput.x * cameraRight + moveInput.y * cameraForward;
        float   moveSpeed     = m_PlayerMoveSpeed * Time.deltaTime;
        Vector3 moveAmount    = moveSpeed * moveDirection;
        transform.Translate(moveAmount);
    }
}
