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
        private InputAction    m_MoveAction;
        private InputAction    m_InteractAction;
        private InputAction    m_ToggleInventoryAction;
        private InputAction    m_AttackAction;
        private InputAction    m_SkipDialogAction;

        void Start()
        {
            m_WorldInputMap         = m_InputAsset.FindActionMap("Player_World");
            m_MoveAction            = m_WorldInputMap.FindAction("Move");
            m_InteractAction        = m_WorldInputMap.FindAction("Interact");
            m_ToggleInventoryAction = m_WorldInputMap.FindAction("Toggle Inventory");
            m_AttackAction          = m_WorldInputMap.FindAction("Attack");

            m_DialogInputMap   = m_InputAsset.FindActionMap("Player_Dialog");
            m_SkipDialogAction = m_DialogInputMap.FindAction("Skip");
        }

        //
        // TODO:
        // Compute input onces and write a read-only state which the code queries? Not that useful, but
        // better in cases where we do many queries to these (which doesn't seem to be the case, so maybe
        // we do nothing).
        //
        // Another experiment I kind of want to try is funneling these as events? I guess it might be overkill for
        // no good reasons, these are arlready eventful.. and it's really easy to do it without events anyway and
        // I consider events to be a last resort solution.
        //

        public Vector2 MoveInput           => m_MoveAction?.ReadValue<Vector2>().normalized ?? Vector2.zero;
        public bool    IsInteracting       => m_InteractAction?.WasPressedThisFrame() ?? false;
        public bool    IsTogglingInventory => m_ToggleInventoryAction?.WasPressedThisFrame() ?? false;
        public bool    IsAttacking         => m_AttackAction?.WasPressedThisFrame() ?? false;
        public bool    IsSkippingDialog    => m_SkipDialogAction?.WasPressedThisFrame() ?? false;

        public void SetInputState(PlayerState section, bool value)
        {
            if(section != null)
            {
                System.Type    sectionType = section.GetType();
                InputActionMap actionMap   = null;

                if(sectionType == typeof(PlayerInWorld))
                {
                    actionMap = m_WorldInputMap;

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
