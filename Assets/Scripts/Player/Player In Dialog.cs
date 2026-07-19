namespace Survivor.Player
{
    public class PlayerInDialog : PlayerState
    {
        public override void OnEnter(PlayerController controller)
        {
            // No-Op
        }

        public override void OnUpdate(PlayerInputProvider inputs, PlayerAnimator animator, PlayerBehavior behavior, DialogSystem dialogSystem, PlayerController controller)
        {
            bool isDialogEnded = dialogSystem.UpdateDialog(inputs.IsSkippingDialog);
            if(isDialogEnded)
            {
                controller.ChangeState(new PlayerInWorld());
            }
        }
    }
}