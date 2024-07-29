using Godot;
using Theadventureofhink.game_state;

public class DashingState : PlayerState
{
    public DashingState(PlatformController controller) : base(controller, "Dashing")
    {
    }

    public override void Enter()
    {
        controller.DashSound.Play();
        controller.DashDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        // If no direction is held by player, just dash forward
        if (controller.DashDirection == Vector2.Zero)
        {
            controller.DashDirection =
                new Vector2(controller.CurrentPlayerDirection == PlatformController.PlayerDirection.Right ? 1 : -1, 0);
        }

        controller.DashTimeLeft = controller.DashTime;
        controller.DashesLeft--;
        controller.EmitSignal(PlatformController.SignalName.PlayerDashStarted, controller.DashDirection);
    }

    public override void Exit()
    {
        controller.EmitSignal(PlatformController.SignalName.PlayerDashStopped);
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        controller.Player.Velocity = controller.DashDirection * controller.DashSpeed;
        controller.Player.MoveAndSlide();

        controller.DashTimeLeft -= (float)delta;
        if (controller.DashTimeLeft <= 0)
        {
            controller.Player.Velocity *= 0.3f;
            controller.ChangeState(new JumpingState(controller));
        }
    }
}