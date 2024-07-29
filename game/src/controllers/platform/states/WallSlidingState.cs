using Godot;
using Theadventureofhink.game_state;

public class WallSlidingState : PlayerState
{
    public WallSlidingState(PlatformController controller) : base(controller, "WallSliding") { }

    public override void Enter()
    {
        controller.AnimatedSprite.Play("grabbing_wall");
    }

    public override void Exit() { }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        float direction = Input.GetAxis("move_left", "move_right");

        controller.Player.Velocity = new Vector2(controller.Player.Velocity.X, 
            Mathf.Lerp(controller.Player.Velocity.Y, 50.0f, 0.1f));
        controller.Player.Velocity = new Vector2(direction, controller.Player.Velocity.Y);

        controller.Player.MoveAndSlide();

        if (controller.Player.IsOnWall())
        {
            if ((controller.Player.GetWallNormal().X * direction) > 0)
            {
                controller.CoyoteTimeFromWallLeft = controller.CoyoteTimeLimit;
                controller.ChangeState(new FallingState(controller));
            }
            if (direction == 0)
            {
                controller.CoyoteTimeFromWallLeft = controller.CoyoteTimeLimit;
                controller.Player.Velocity = new Vector2(0, controller.Player.Velocity.Y);
                controller.ChangeState(new FallingState(controller));
            }

            if (Input.IsActionJustPressed("jump"))
            {
                controller.TriggerJump(PlatformController.JumpSource.Wall);
                Vector2 jumpDirection = new Vector2(controller.Player.GetWallNormal().X, -1);
                controller.Player.Velocity = jumpDirection.Normalized() * controller.JumpVelocity;
                controller.ChangeState(new JumpingState(controller));
            }

            if (Input.IsActionJustPressed("grab_wall") && controller.WallGrabTimeLeft > 0)
            {
                controller.ChangeState(new GrabbingWallState(controller));
            }
            if (controller.Player.IsOnFloor())
            {
                controller.ChangeState(new IdleState(controller));
            }
        }
        else
        {
            controller.CoyoteTimeFromWallLeft = controller.CoyoteTimeLimit;
            controller.ChangeState(new FallingState(controller));
        }
    }
}