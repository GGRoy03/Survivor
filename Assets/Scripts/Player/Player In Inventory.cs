namespace Survivor.Player
{
    public class PlayerInInventory : PlayerState
    {
        public override void OnEnter(PlayerController controller)
        {
            // No-Op
        }

        public override void OnUpdate(PlayerInputProvider inputs, PlayerBehavior behavior, DialogSystem dialog, PlayerController controller)
        {
            if(inputs.IsTogglingInventory)
            {
                controller.ChangeState(new PlayerInWorld());
            }
        }

        public override AnimationInfo OnAnimate()
        {
            return new AnimationInfo();
        }
    }
}