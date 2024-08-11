using Godot;
using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform.states;

public class WallSlidingState(PlatformController controller) : PlayerState("WallSliding", controller)
{
    public override void Enter()
    {
        Controller.AnimatedSprite.Play("grabbing_wall");
    }

    public override void Exit()
    {
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        float direction = Input.GetAxis("move_left", "move_right");

        Controller.Player.Velocity = new Vector2(Controller.Player.Velocity.X,
            Mathf.Lerp(Controller.Player.Velocity.Y, 50.0f, 0.1f));
        Controller.Player.Velocity = new Vector2(direction, Controller.Player.Velocity.Y);

        Controller.Player.MoveAndSlide();

        if (Controller.Player.IsOnWall())
        {
            if ((Controller.Player.GetWallNormal().X * direction) > 0)
            {
                Controller.CoyoteTimeFromWallLeft = Controller.CoyoteTimeLimit;
                Controller.ChangeState(new FallingState(Controller));
            }

            if (direction == 0)
            {
                Controller.CoyoteTimeFromWallLeft = Controller.CoyoteTimeLimit;
                Controller.Player.Velocity = new Vector2(0, Controller.Player.Velocity.Y);
                Controller.ChangeState(new FallingState(Controller));
            }

            if (Input.IsActionJustPressed("jump") && playerSkillsState.CanWallJump.Value())
            {
                Controller.TriggerJump(PlatformController.JumpSource.Wall);
                var jumpDirection = new Vector2(Controller.Player.GetWallNormal().X, -1);
                Controller.Player.Velocity = jumpDirection.Normalized() * Controller.JumpVelocity;
            }

            if (Input.IsActionJustPressed("grab_wall") && Controller.WallGrabTimeLeft > 0 &&
                playerSkillsState.CanClimbWalls.Value())
            {
                Controller.ChangeState(new GrabbingWallState(Controller));
            }

            if (Controller.Player.IsOnFloor())
            {
                Controller.ChangeState(new IdleState(Controller));
            }
        }
        else
        {
            Controller.CoyoteTimeFromWallLeft = Controller.CoyoteTimeLimit;
            Controller.ChangeState(new FallingState(Controller));
        }
    }
}