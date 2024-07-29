using Godot;
using Theadventureofhink.game_state;

public class IdleState : PlayerState
{
    public IdleState(PlatformController controller) : base(controller, "Idle")
    {
    }

    public override void Enter()
    {
        controller.WallGrabTimeLeft = controller.WallGrabTimeLimit;
        controller.AnimatedSprite.Play("idle");
    }

    public override void Exit()
    {
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        float direction = Input.GetAxis("move_left", "move_right");

        if (!controller.Player.IsOnFloor())
        {
            controller.JumpsLeft = 0;
            controller.DashesLeft = controller.NumDashes;
            controller.CoyoteTimeFromGroundLeft = controller.CoyoteTimeLimit;
            controller.EmitSignal(PlatformController.SignalName.PlayerStoppedMovingOnGround);
            controller.ChangeState(new FallingState(controller));
        }
        else
        {
            bool isJumpAllowed = controller.Player.IsOnFloor() || controller.TimeSinceNoGround < 0.1;

            if (Input.IsActionJustPressed("jump") && isJumpAllowed)
            {
                controller.EmitSignal(PlatformController.SignalName.PlayerStoppedMovingOnGround);
                controller.TriggerJump(PlatformController.JumpSource.Ground);
                controller.Player.Velocity = new Vector2(direction * controller.Speed, -controller.JumpVelocity);
            }
            else
            {
                if (direction != 0.0f)
                {
                    controller.AnimatedSprite.Play("walk");
                    controller.EmitSignal(PlatformController.SignalName.PlayerStartedMovingOnGround);
                }
                else
                {
                    controller.AnimatedSprite.Play("idle");
                    controller.EmitSignal(PlatformController.SignalName.PlayerStoppedMovingOnGround);
                }

                if (direction != 0.0f)
                {
                    controller.Player.Velocity =
                        new Vector2(Mathf.Lerp(controller.Player.Velocity.X, direction * controller.Speed, 0.3f),
                            controller.Player.Velocity.Y);
                }
                else
                {
                    controller.Player.Velocity =
                        new Vector2(Mathf.MoveToward(controller.Player.Velocity.X, 0, controller.Speed),
                            controller.Player.Velocity.Y);
                }

                controller.Player.MoveAndSlide();
            }
        }
    }
}