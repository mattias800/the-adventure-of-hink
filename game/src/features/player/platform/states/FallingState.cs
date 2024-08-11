using Godot;
using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform.states;

public class FallingState(PlatformController controller) : PlayerState("Falling", controller)
{
    public override void Enter()
    {
        Controller.AnimatedSprite.Play("fall");
    }

    public override void Exit()
    {
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        Controller.TimeUntilJumpVelocityResetAllowed -= (float)delta;
        Controller.TimeUntilJumpHorizontalControl -= (float)delta;
        Controller.TimeSinceNoGround += (float)delta;
        Controller.TimeUntilWallGrabPossible -= (float)delta;
        Controller.Player.Velocity = new Vector2(Controller.Player.Velocity.X,
            Mathf.Min(Controller.Player.Velocity.Y + Controller.Gravity * (float)delta, Controller.MaxFallSpeed));

        if (Controller.CoyoteTimeFromGroundLeft > 0)
        {
            Controller.CoyoteTimeFromGroundLeft -= (float)delta;
        }

        if (Controller.CoyoteTimeFromWallLeft > 0)
        {
            Controller.CoyoteTimeFromWallLeft -= (float)delta;
        }

        float direction = Input.GetAxis("move_left", "move_right");

        if (Input.IsActionJustPressed("jump") && Controller.CoyoteTimeFromGroundLeft > 0.0f)
        {
            Controller.TriggerJump(PlatformController.JumpSource.Ground);
            Controller.Player.Velocity = new Vector2(direction * Controller.Speed, -Controller.JumpVelocity);
        }
        else if (Input.IsActionJustPressed("jump") && Controller.CoyoteTimeFromWallLeft > 0.0f &&
                 playerSkillsState.CanWallJump.Value())
        {
            Controller.TriggerJump(PlatformController.JumpSource.Wall);
            Controller.Player.Velocity = Controller.GetWallJumpDirection(Controller.NormalForLastWallTouched) *
                                         Controller.JumpVelocity;
        }
        else if (Input.IsActionJustPressed("jump") && Controller.JumpsLeft > 0 &&
                 playerSkillsState.CanDoubleJump.Value()
                )
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

            if (Controller.Player.IsOnFloor())
            {
                Controller.LandSound.Play();
                Controller.SpawnDustBoom();
                Controller.ChangeState(new IdleState(Controller));
            }
            else if (Controller.Player.IsOnWall())
            {
                if (Input.IsActionPressed("grab_wall") && Controller.TimeUntilWallGrabPossible <= 0.0f &&
                    playerSkillsState.CanClimbWalls.Value())
                {
                    Controller.VelocityIntoWall = Controller.Player.GetWallNormal().X * -1;

                    var ray = Controller.VelocityIntoWall > 0.0f
                        ? Controller.WallRayCastRight
                        : Controller.WallRayCastLeft;

                    if (Controller.WallClimbableProvider.IsWallClimbable(ray, Controller.VelocityIntoWall))
                    {
                        if (Controller.WallGrabTimeLeft >= 0.0f)
                        {
                            Controller.ChangeState(new GrabbingWallState(Controller));
                        }
                        else
                        {
                            Controller.ChangeState(new WallSlidingState(Controller));
                        }
                    }
                }
                else if (direction != 0)
                {
                    // If this is enabled, player will slide down walls even when not holding grab button. 
                    // Controller.ChangeState(new WallSlidingState(Controller));
                }
            }
        }
    }
}