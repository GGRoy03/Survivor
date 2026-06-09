using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCore : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionAsset m_PlayerInputMap;
                     private InputAction      m_WorldMoveAction;
                     private InputAction      m_WorldInteractAction;

    [Header("Interaction")]
    [SerializeField] private float            m_InteractionRange;
    [SerializeField] private LayerMask        m_InteractionMask;

    [Header("Stats")]
    [SerializeField] private float m_MaxHealth;
    [SerializeField] private float m_MaxStamina;
    [SerializeField] private float m_MaxHunger;
    [SerializeField] private float m_HungerDecreaseRate;
                     private float m_CurrentHealth;
                     private float m_CurrentStamina;
                     private float m_CurrentHunger;

    public float     Health     => m_CurrentHealth;
    public float     Hunger     => m_CurrentHunger;
    public float     Stamina    => m_CurrentStamina;
    public float     MaxHealth  => m_MaxHealth;
    public float     MaxStamina => m_MaxStamina;
    public float     MaxHunger  => m_MaxHunger;
    public GameState State      => m_CurrentState;

    //
    // NOTE:
    // x) I mean this is just not good.
    //

    private PlayerMovement m_PlayerMovement;

    public enum GameState
    {
        World  = 0,
        Dialog = 1,
    }
    private GameState m_CurrentState;

    private void Start()
    {
        m_CurrentState   = GameState.World;

        m_PlayerMovement = GetComponent<PlayerMovement>();

        var m_PlayerInWorldInput = m_PlayerInputMap.FindActionMap("PlayerInWorld");
        m_WorldMoveAction     = m_PlayerInWorldInput.FindAction("Move");
        m_WorldInteractAction = m_PlayerInWorldInput.FindAction("Interact");

        m_CurrentHealth  = m_MaxHealth;
        m_CurrentStamina = m_MaxStamina;
        m_CurrentHunger  = m_MaxHunger;
    }

    private void Update()
    {
        m_CurrentHunger -= m_HungerDecreaseRate;

        switch (m_CurrentState)
        {
            case GameState.World:
            {
                Vector2 moveInput = m_WorldMoveAction.ReadValue<Vector2>();
                {
                    Vector3 moveAmount = m_PlayerMovement.MoveOnGround(moveInput);
                    transform.Translate(moveAmount);
                }

                bool isInteracting = m_WorldInteractAction.WasPressedThisFrame();
                if(isInteracting)
                {
                    m_CurrentState = GameState.Dialog;
                }

            } break;

            case GameState.Dialog:
            {
                
            } break;
        }
    }
}
