using Godot;
using Theadventureofhink.game_state;

public class FallingState : PlayerState
{
    public FallingState(PlatformController controller) : base(controller, "Falling")
    {
    }

    public override void Enter()
    {
        controller.AnimatedSprite.Play("fall");
    }

    public override void Exit()
    {
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        controller.TimeUntilJumpVelocityResetAllowed -= (float)delta;
        controller.TimeUntilJumpHorizontalControl -= (float)delta;
        controller.TimeSinceNoGround += (float)delta;
        controller.TimeUntilWallGrabPossible -= (float)delta;
        controller.Player.Velocity = new Vector2(controller.Player.Velocity.X,
            Mathf.Min(controller.Player.Velocity.Y + controller.Gravity * (float)delta, controller.MaxFallSpeed));

        if (controller.CoyoteTimeFromGroundLeft > 0)
        {
            controller.CoyoteTimeFromGroundLeft -= (float)delta;
        }

        if (controller.CoyoteTimeFromWallLeft > 0)
        {
            controller.CoyoteTimeFromWallLeft -= (float)delta;
        }

        float direction = Input.GetAxis("move_left", "move_right");

        if (Input.IsActionJustPressed("jump") && controller.CoyoteTimeFromGroundLeft > 0.0f)
        {
            controller.TriggerJump(PlatformController.JumpSource.Ground);
            controller.Player.Velocity = new Vector2(direction * controller.Speed, -controller.JumpVelocity);
        }
        else if (Input.IsActionJustPressed("jump") && controller.CoyoteTimeFromWallLeft > 0.0f &&
                 playerSkillsState.CanWallJump.Value())
        {
            controller.TriggerJump(PlatformController.JumpSource.Wall);
            controller.Player.Velocity = controller.GetWallJumpDirection(controller.NormalForLastWallTouched) *
                                         controller.JumpVelocity;
        }
        else if (Input.IsActionJustPressed("jump") && controller.JumpsLeft > 0 &&
                 playerSkillsState.CanDoubleJump.Value()
                )
        {
            controller.TriggerJump(PlatformController.JumpSource.Air);
            controller.Player.Velocity = new Vector2(controller.Player.Velocity.X, -controller.JumpVelocity);
        }
        else if (Input.IsActionJustPressed("dash") && controller.DashesLeft > 0 && playerSkillsState.CanDash.Value())
        {
            controller.TriggerDash();
        }
        else
        {
            if (controller.TimeUntilJumpHorizontalControl <= 0)
            {
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

            if (controller.Player.IsOnFloor())
            {
                controller.LandSound.Play();
                controller.SpawnDustBoom();
                controller.ChangeState(new IdleState(controller));
            }
            else if (controller.Player.IsOnWall())
            {
                if (Input.IsActionPressed("grab_wall") && controller.TimeUntilWallGrabPossible <= 0.0f &&
                    playerSkillsState.CanClimbWalls.Value())
                {
                    controller.VelocityIntoWall = controller.Player.GetWallNormal().X * -1;
                    if (controller.WallGrabTimeLeft >= 0.0f)
                    {
                        controller.ChangeState(new GrabbingWallState(controller));
                    }
                    else
                    {
                        controller.ChangeState(new WallSlidingState(controller));
                    }
                }
                else if (direction != 0)
                {
                    controller.ChangeState(new WallSlidingState(controller));
                }
            }
        }
    }
}