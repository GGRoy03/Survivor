using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Survivor.Player
{
    public class PlayerInputProvider : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InputActionAsset m_InputAsset;

        private InputActionMap m_WorldInputMap;
        private InputActionMap m_DialogInputMap;
        private InputActionMap m_AllInputMap;
        private InputAction    m_MoveAction;
        private InputAction    m_InteractAction;
        private InputAction    m_PointerPositionAction;
        private InputAction    m_ToggleInventoryAction;
        private InputAction    m_AttackAction;
        private InputAction    m_SkipDialogAction;

        void Start()
        {
            m_AllInputMap    = m_InputAsset.FindActionMap("Player_All");
            m_WorldInputMap  = m_InputAsset.FindActionMap("Player_World");
            m_DialogInputMap = m_InputAsset.FindActionMap("Player_Dialog");

            m_ToggleInventoryAction = m_AllInputMap.FindAction("Toggle Inventory");

            m_MoveAction            = m_WorldInputMap.FindAction("Move");
            m_InteractAction        = m_WorldInputMap.FindAction("Interact");
            m_AttackAction          = m_WorldInputMap.FindAction("Attack");
            m_PointerPositionAction = m_WorldInputMap.FindAction("Pointer Position");

            m_SkipDialogAction = m_DialogInputMap.FindAction("Skip");
        }

        //
        // TODO:
        // Compute input onces and write a read-only state which the code queries? Not that useful right now, but
        // better in cases where we do many queries to these (which doesn't seem to be the case, so maybe
        // we do nothing).
        //

        public Vector2 MoveInput       => m_MoveAction?.ReadValue<Vector2>().normalized ?? Vector2.zero;
        public Vector2 PointerPosition => m_PointerPositionAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool    IsInteracting       => m_InteractAction?.WasPressedThisFrame() ?? false;
        public bool    IsTogglingInventory => m_ToggleInventoryAction?.WasPressedThisFrame() ?? false;
        public bool    IsAttacking         => m_AttackAction?.WasPressedThisFrame() ?? false;
        public bool    IsSkippingDialog    => m_SkipDialogAction?.WasPressedThisFrame() ?? false;
       
        //
        // NOTE:
        // I don't even know if this is useful.
        //

        public void SetInputState(PlayerState section, bool value)
        {
            if(section != null)
            {
                System.Type    sectionType = section.GetType();
                InputActionMap actionMap   = null;

                //
                // NOTE:
                // Kind of annoying, because it's not exhaustive.
                //

                if(sectionType == typeof(PlayerInWorld))
                {
                    actionMap = m_WorldInputMap;
                }
                else if(sectionType == typeof(PlayerInDialog))
                {
                    actionMap = m_DialogInputMap;
                }

                if(actionMap != null)
                {
                    if(value)
                    {
                        actionMap.Enable();
                    }
                    else
                    {
                        actionMap.Disable();
                    }
                }
            }
        }

    }
}
