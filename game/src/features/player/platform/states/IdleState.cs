using Godot;
using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform.states;

public class IdleState(PlatformController controller) : PlayerState("Idle", controller)
{
    public override void Enter()
    {
        Controller.WallGrabTimeLeft = Controller.WallGrabTimeLimit;
        Controller.AnimatedSprite.Play("idle");
    }

    public override void Exit()
    {
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        float direction = Input.GetAxis("move_left", "move_right");

        if (!Controller.Player.IsOnFloor())
        {
            Controller.JumpsLeft = 0;
            Controller.DashesLeft = Controller.NumDashes;
            Controller.CoyoteTimeFromGroundLeft = Controller.CoyoteTimeLimit;
            Controller.EmitSignal(PlatformController.SignalName.PlayerStoppedMovingOnGround);
            Controller.ChangeState(new FallingState(Controller));
        }
        else
        {
            bool isJumpAllowed = Controller.Player.IsOnFloor() || Controller.TimeSinceNoGround < 0.1;

            if (Input.IsActionJustPressed("jump") && isJumpAllowed)
            {
                Controller.EmitSignal(PlatformController.SignalName.PlayerStoppedMovingOnGround);
                Controller.TriggerJump(PlatformController.JumpSource.Ground);
                Controller.Player.Velocity = new Vector2(direction * Controller.Speed, -Controller.JumpVelocity);
            }
            else
            {
                if (direction != 0.0f)
                {
                    Controller.AnimatedSprite.Play("walk");
                    Controller.EmitSignal(PlatformController.SignalName.PlayerStartedMovingOnGround);
                }
                else
                {
                    Controller.AnimatedSprite.Play("idle");
                    Controller.EmitSignal(PlatformController.SignalName.PlayerStoppedMovingOnGround);
                }

                if (direction != 0.0f)
                {
                    Controller.Player.Velocity =
                        new Vector2(Mathf.Lerp(Controller.Player.Velocity.X, direction * Controller.Speed, 0.3f),
                            Controller.Player.Velocity.Y);
                }
                else
                {
                    Controller.Player.Velocity =
                        new Vector2(Mathf.MoveToward(Controller.Player.Velocity.X, 0, Controller.Speed),
                            Controller.Player.Velocity.Y);
                }

                Controller.Player.MoveAndSlide();
            }
        }
    }
}