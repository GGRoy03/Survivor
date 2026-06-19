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
        private InputAction    m_SkipDialogAction;

        void Start()
        {
            m_WorldInputMap  = m_InputAsset.FindActionMap("Player_World");
            m_MoveAction     = m_WorldInputMap.FindAction("Move");
            m_InteractAction = m_WorldInputMap.FindAction("Interact");

            m_DialogInputMap   = m_InputAsset.FindActionMap("Player_Dialog");
            m_SkipDialogAction = m_DialogInputMap.FindAction("Skip");
        }

        public Vector2 MoveInput        => m_MoveAction?.ReadValue<Vector2>().normalized ?? Vector2.zero;
        public bool    IsInteracting    => m_InteractAction?.WasPressedThisFrame() ?? false;
        public bool    IsSkippingDialog => m_SkipDialogAction?.WasPressedThisFrame() ?? false;

        public void SetInputState(PlayerState section, bool value)
        {
            switch(section)
            {
                case PlayerState.World:  SetInputMapState(m_WorldInputMap, value);  break;
                case PlayerState.Dialog: SetInputMapState(m_DialogInputMap, value); break;
            }
        }

        private void SetInputMapState(InputActionMap inputMap, bool value)
        {
            if(inputMap != null)
            {
                if (value) inputMap.Enable();
                else       inputMap.Disable();
            }
        }
    }
}
