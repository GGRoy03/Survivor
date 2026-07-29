using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InputActionAsset m_InputAsset;

    //
    // Input State
    //

    public struct InputAlways
    {
        public bool IsInventoryToggled;
        public bool IsPauseMenuToggled;
    }

    public struct InputWorld
    {
        public  Vector2 Move;
        public  Vector2 PointerPosition;
        public  bool    IsInteracting;
        public  bool    IsAttacking;
    }

    public struct InputDialog
    {
        public  bool IsSkipping;
    }

    public InputAlways Always { get; private set; }
    public InputWorld  World { get; private set; }
    public InputDialog Dialog { get; private set; }

    //
    // Unity Hooks
    //

    private InputActionMap m_WorldInputMap;
    private InputActionMap m_DialogInputMap;
    private InputActionMap m_AlwaysInputMap;

    private InputAction m_ToggleInventoryAction;
    private InputAction m_TogglePauseMenuAction;

    private InputAction m_MoveAction;
    private InputAction m_InteractAction;
    private InputAction m_PointerPositionAction;
    private InputAction m_AttackAction;

    private InputAction m_SkipDialogAction;

    void Start()
    {
        //
        // Always Inputs
        //

        m_AlwaysInputMap        = FindActionMap("Always");
        m_ToggleInventoryAction = FindActionInMap(m_AlwaysInputMap, "Toggle Inventory");
        m_TogglePauseMenuAction = FindActionInMap(m_AlwaysInputMap, "Toggle Pause Menu");

        //
        // World Inputs
        //

        m_WorldInputMap         = FindActionMap("InWorld");
        m_MoveAction            = FindActionInMap(m_AlwaysInputMap, "Move");
        m_InteractAction        = FindActionInMap(m_AlwaysInputMap, "Interact");
        m_AttackAction          = FindActionInMap(m_AlwaysInputMap, "Attack");
        m_PointerPositionAction = FindActionInMap(m_AlwaysInputMap, "Pointer Position");

        //
        // Dialog Inputs
        //

        m_DialogInputMap   = FindActionMap("InDialog");
        m_SkipDialogAction = FindActionInMap(m_DialogInputMap, "Skip");

    }

    private void Update()
    {
        Always = new InputAlways()
        {
            IsInventoryToggled = ActionToBool(m_ToggleInventoryAction),
            IsPauseMenuToggled = ActionToBool(m_TogglePauseMenuAction),
        };

        World = new InputWorld()
        {
            Move            = ActionToVector2(m_MoveAction),
            PointerPosition = ActionToVector2(m_PointerPositionAction),
            IsInteracting   = ActionToBool(m_InteractAction),
            IsAttacking     = ActionToBool(m_AttackAction),
        };

        Dialog = new InputDialog()
        {
            IsSkipping = ActionToBool(m_SkipDialogAction),
        };
    }

    private bool ActionToBool(InputAction action)
    {
        bool result = action?.WasPressedThisFrame() ?? false;
        return result;
    }

    private Vector2 ActionToVector2(InputAction action)
    {
        var result = action?.ReadValue<Vector2>() ?? Vector2.zero;
        return result;
    }

    private InputActionMap FindActionMap(string name)
    {
        InputActionMap result = null;

        if(m_InputAsset != null)
        {
            result = m_InputAsset.FindActionMap(name);
        }

        return result;
    }

    private InputAction FindActionInMap(InputActionMap map, string name)
    {
        var result = map?.FindAction(name);
        return result;
    }

    //
    // Input State
    //

    public enum InputContext
    {
        World  = 0,
        Dialog = 1,
    }

    public void SetActiveContext(InputContext context)
    {
        SetInputMapState(m_WorldInputMap , InputContext.World , context);
        SetInputMapState(m_DialogInputMap, InputContext.Dialog, context);
    }

    private void SetInputMapState(InputActionMap inputMap, InputContext srcContext, InputContext dstContext)
    {
        if(inputMap != null)
        {
            if(srcContext == dstContext)
            {
                inputMap.Enable();
            }
            else
            {
                inputMap.Disable();
            }
        }
    }
}