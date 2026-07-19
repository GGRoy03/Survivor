using Survivor.Inventory;

namespace Survivor.Player
{
    public class PlayerInInventory : PlayerState
    {
        public override void OnEnter(PlayerController controller)
        {
            InventorySystemUI.Instance.SetVisibility(true);
        }

        public override void OnUpdate(PlayerInputProvider inputs, PlayerAnimator animator, PlayerBehavior behavior, DialogSystem dialog, PlayerController controller)
        {
            if(inputs.IsTogglingInventory)
            {
                InventorySystemUI.Instance.SetVisibility(false);

                controller.ChangeState(new PlayerInWorld());
            }
        }
    }
}