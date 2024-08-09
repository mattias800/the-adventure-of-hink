using Godot;
using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform.states;

public class JumpingState : PlayerState
{
    public JumpingState(PlatformController controller) : base(controller, "Jumping")
    {
    }

    public override void Enter()
    {
        Controller.AnimatedSprite.Play("jump");
    }

    public override void Exit()
    {
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        Controller.TimeUntilJumpVelocityResetAllowed -= (float)delta;
        Controller.TimeUntilJumpHorizontalControl -= (float)delta;
        Controller.Player.Velocity += new Vector2(0, Controller.Gravity * (float)delta);
        Controller.TimeSinceNoGround += (float)delta;
        Controller.TimeUntilWallGrabPossible -= (float)delta;

        if (!Input.IsActionPressed("jump") && Controller.Player.Velocity.Y < Controller.JumpReleaseVelocity &&
            Controller.TimeUntilJumpVelocityResetAllowed <= 0)
        {
            Controller.Player.Velocity = new Vector2(Controller.Player.Velocity.X, -50);
            Controller.ChangeState(new FallingState(Controller));
        }
        else if (playerSkillsState.CanDoubleJump.Value() && Input.IsActionJustPressed("jump") &&
                 Controller.JumpsLeft > 0)
        {
            Controller.TriggerJump(PlatformController.JumpSource.Air);
            Controller.Player.Velocity = new Vector2(Controller.Player.Velocity.X, -Controller.JumpVelocity);
        }
        else if (Input.IsActionJustPressed("dash") && Controller.DashesLeft > 0 && playerSkillsState.CanDash.Value())
        {
            Controller.TriggerDash();
        }
        else
        {
            if (Controller.TimeUntilJumpHorizontalControl <= 0)
            {
                float direction = Input.GetAxis("move_left", "move_right");
                if (direction != 0)
                {
                    Controller.AddVelocityX(direction * Controller.JumpHorizontalSpeed);
                }
                else
                {
                    Controller.Player.Velocity = new Vector2(Mathf.Lerp(Controller.Player.Velocity.X, 0.0f, 0.05f),
                        Controller.Player.Velocity.Y);
                }
            }

            Controller.Player.MoveAndSlide();

            if (Controller.Player.Velocity.Y > 0)
            {
                Controller.ChangeState(new FallingState(Controller));
            }

            if (Controller.Player.IsOnFloor())
            {
                Controller.ChangeState(new IdleState(Controller));
            }

            if (Controller.Player.IsOnWall() && Controller.TimeUntilWallGrabPossible <= 0.0 &&
                Input.IsActionPressed("grab_wall") && playerSkillsState.CanClimbWalls.Value())
            {
                Controller.VelocityIntoWall = Controller.Player.GetWallNormal().X * -1;
                if (Controller.WallGrabTimeLeft >= 0.0)
                {
                    Controller.ChangeState(new GrabbingWallState(Controller));
                }
                else
                {
                    Controller.ChangeState(new WallSlidingState(Controller));
                }
            }
        }
    }
}