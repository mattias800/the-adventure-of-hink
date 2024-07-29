using Godot;
using Theadventureofhink.game_state;

public class JumpingState : PlayerState
{
    public JumpingState(PlatformController controller) : base(controller, "Jumping")
    {
    }

    public override void Enter()
    {
        controller.AnimatedSprite.Play("jump");
    }

    public override void Exit()
    {
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        controller.TimeUntilJumpVelocityResetAllowed -= (float)delta;
        controller.TimeUntilJumpHorizontalControl -= (float)delta;
        controller.Player.Velocity += new Vector2(0, controller.Gravity * (float)delta);
        controller.TimeSinceNoGround += (float)delta;
        controller.TimeUntilWallGrabPossible -= (float)delta;

        if (!Input.IsActionPressed("jump") && controller.Player.Velocity.Y < controller.JumpReleaseVelocity &&
            controller.TimeUntilJumpVelocityResetAllowed <= 0)
        {
            controller.Player.Velocity = new Vector2(controller.Player.Velocity.X, -50);
            controller.ChangeState(new FallingState(controller));
        }
        else if (Input.IsActionJustPressed("jump") && controller.JumpsLeft > 0)
        {
            controller.TriggerJump(PlatformController.JumpSource.Air);
            controller.Player.Velocity = new Vector2(controller.Player.Velocity.X, -controller.JumpVelocity);
        }
        else if (Input.IsActionJustPressed("dash") && controller.DashesLeft > 0)
        {
            controller.TriggerDash();
        }
        else
        {
            if (controller.TimeUntilJumpHorizontalControl <= 0)
            {
                float direction = Input.GetAxis("move_left", "move_right");
                if (direction != 0)
                {
                    controller.AddVelocityX(direction * controller.JumpHorizontalSpeed);
                }
                else
                {
                    controller.Player.Velocity = new Vector2(Mathf.Lerp(controller.Player.Velocity.X, 0.0f, 0.05f),
                        controller.Player.Velocity.Y);
                }
            }

            controller.Player.MoveAndSlide();

            if (controller.Player.Velocity.Y > 0)
            {
                controller.ChangeState(new FallingState(controller));
            }

            if (controller.Player.IsOnFloor())
            {
                controller.ChangeState(new IdleState(controller));
            }

            if (controller.Player.IsOnWall() && controller.TimeUntilWallGrabPossible <= 0.0 &&
                Input.IsActionPressed("grab_wall"))
            {
                controller.VelocityIntoWall = controller.Player.GetWallNormal().X * -1;
                if (controller.WallGrabTimeLeft >= 0.0)
                {
                    controller.ChangeState(new GrabbingWallState(controller));
                }
                else
                {
                    controller.ChangeState(new WallSlidingState(controller));
                }
            }
        }
    }
}