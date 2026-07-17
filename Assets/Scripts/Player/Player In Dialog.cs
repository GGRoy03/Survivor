namespace Survivor.Player
{
    public class PlayerInDialog : PlayerState
    {
        public override void OnEnter(PlayerController controller)
        {
            // No-Op
        }

        public override void OnUpdate(PlayerInputProvider inputs, PlayerBehavior behavior, DialogSystem dialogSystem, PlayerController controller)
        {
            bool isDialogEnded = dialogSystem.UpdateDialog(inputs.IsSkippingDialog);
            if(isDialogEnded)
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