using Godot;
using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform.states;

public class DashingState(PlatformController controller) : PlayerState(controller, "Dashing")
{
    public override void Enter()
    {
        Controller.DashSound.Play();
        Controller.DashDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        // If no direction is held by player, just dash forward
        if (Controller.DashDirection == Vector2.Zero)
        {
            Controller.DashDirection =
                new Vector2(Controller.CurrentPlayerDirection == PlatformController.PlayerDirection.Right ? 1 : -1, 0);
        }

        Controller.DashTimeLeft = Controller.DashTime;
        Controller.DashesLeft--;
        Controller.EmitSignal(PlatformController.SignalName.PlayerDashStarted, Controller.DashDirection);
    }

    public override void Exit()
    {
        Controller.EmitSignal(PlatformController.SignalName.PlayerDashStopped);
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        Controller.Player.Velocity = Controller.DashDirection * Controller.DashSpeed;
        Controller.Player.MoveAndSlide();

        Controller.DashTimeLeft -= (float)delta;
        if (Controller.DashTimeLeft <= 0)
        {
            Controller.Player.Velocity *= 0.3f;
            Controller.ChangeState(new JumpingState(Controller));
        }
    }
}