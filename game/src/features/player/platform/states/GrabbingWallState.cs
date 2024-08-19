using Godot;
using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform.states;

public class GrabbingWallState(PlatformController controller) : PlayerState("GrabbingWall", controller)
{
    public override void Enter()
    {
        Controller.AnimatedSprite.Play("grabbing_wall");
        Controller.GrabWallSound.Play();
        Controller.Player.Velocity = new Vector2(Controller.Player.Velocity.X, 0);
        Controller.WallGrabTimeLeft = Controller.WallGrabTimeLimit;
    }

    public override void Exit()
    {
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        float direction = Input.GetAxis("move_left", "move_right");
        float verticalDirection = Input.GetAxis("move_up", "move_down");

        Controller.DashesLeft = Controller.NumDashes;

        var ray = Controller.VelocityIntoWall > 0.0f
            ? Controller.WallRayCastRight
            : Controller.WallRayCastLeft;

        if (ray.IsColliding() && !Controller.WallClimbableProvider.IsWallClimbable(ray, Controller.VelocityIntoWall))
        {
            // Player is no longer in climbable wall.
            Controller.ChangeState(new FallingState(Controller));
            return;
        }

        if (verticalDirection == 0)
        {
            Controller.AnimatedSprite.Play("grabbing_wall");
        }

        if (Controller.WallGrabTimeLeft <= 0.0f)
        {
            Controller.ChangeState(new WallSlidingState(Controller));
        }
        else if (Input.IsActionJustPressed("jump"))
        {
            Controller.TriggerJump(PlatformController.JumpSource.Wall);
            Controller.Player.Velocity = Controller.GetWallJumpDirection(Controller.Player.GetWallNormal()) *
                                         Controller.JumpVelocity;
            Controller.Player.MoveAndSlide();
        }
        else if (Input.IsActionPressed("move_up"))
        {
            if (Controller.Player.IsOnCeiling())
            {
                Controller.AnimatedSprite.Play("grabbing_wall");
            }
            else
            {
                Controller.WallGrabTimeLeft -= (float)delta;
                Controller.AnimatedSprite.Play("climbing");
            }

            Controller.Player.Velocity =
                new Vector2(
                    Controller.VelocityIntoWall * Controller.JumpVelocity,
                    verticalDirection * Controller.WallClimbSpeed
                );

            Controller.Player.MoveAndSlide();

            if (!Controller.Player.IsOnWall())
            {
                Controller.Player.Velocity = new Vector2(Controller.VelocityIntoWall * Controller.Speed * 0.5f,
                    Controller.Player.Velocity.Y);
                Controller.Player.MoveAndSlide();
                Controller.ChangeState(new IdleState(Controller));
            }
        }
        else if (Input.IsActionPressed("move_down"))
        {
            Controller.WallGrabTimeLeft -= (float)delta;
            
            Controller.AnimatedSprite.Play("climbing");
            Controller.Player.Velocity = new Vector2(Controller.VelocityIntoWall * Controller.JumpVelocity,
                verticalDirection * Controller.WallClimbSpeed);
            Controller.Player.MoveAndSlide();
            if (!Controller.Player.IsOnWall())
            {
                Controller.Player.Velocity = new Vector2(0, Controller.Player.Velocity.Y);
                Controller.Player.MoveAndSlide();
                Controller.ChangeState(new IdleState(Controller));
            }
        }
        else if (!Input.IsActionPressed("grab_wall"))
        {
            Controller.CoyoteTimeFromWallLeft = Controller.CoyoteTimeLimit;
            if ((Controller.Player.GetWallNormal().X * direction) < 0)
            {
                Controller.ChangeState(new WallSlidingState(Controller));
            }
            else
            {
                Controller.ChangeState(new FallingState(Controller));
            }
        }
        else
        {
            Controller.Player.Velocity = new Vector2(Controller.VelocityIntoWall * Controller.JumpVelocity, 0);
            Controller.Player.MoveAndSlide();

            if (!Controller.Player.IsOnWall())
            {
                Controller.Player.Velocity = new Vector2(direction * Controller.JumpHorizontalSpeed,
                    Controller.Player.Velocity.Y);
                Controller.Player.MoveAndSlide();
                Controller.ChangeState(new FallingState(Controller));
            }
        }
    }
}