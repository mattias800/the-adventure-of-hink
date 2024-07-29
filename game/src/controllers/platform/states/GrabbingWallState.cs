using Godot;
using Theadventureofhink.game_state;

public class GrabbingWallState : PlayerState
{
    public GrabbingWallState(PlatformController controller) : base(controller, "GrabbingWall") { }

    public override void Enter()
    {
        controller.AnimatedSprite.Play("grabbing_wall");
        controller.GrabWallSound.Play();
        controller.Player.Velocity = new Vector2(controller.Player.Velocity.X, 0);
        controller.WallGrabTimeLeft = controller.WallGrabTimeLimit;
    }

    public override void Exit() { }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        float direction = Input.GetAxis("move_left", "move_right");
        float verticalDirection = Input.GetAxis("move_up", "move_down");

        controller.DashesLeft = controller.NumDashes;
        controller.WallGrabTimeLeft -= (float)delta;

        if (verticalDirection == 0)
        {
            controller.AnimatedSprite.Play("grabbing_wall");
        }

        if (controller.WallGrabTimeLeft <= 0.0f)
        {
            controller.ChangeState(new WallSlidingState(controller));
        }
        else if (Input.IsActionJustPressed("jump"))
        {
            controller.TriggerJump(PlatformController.JumpSource.Wall);
            controller.Player.Velocity = controller.GetWallJumpDirection(controller.Player.GetWallNormal()) * controller.JumpVelocity;
            controller.Player.MoveAndSlide();
        }
        else if (Input.IsActionPressed("move_up"))
        {
            if (controller.Player.IsOnCeiling())
            {
                controller.AnimatedSprite.Play("grabbing_wall");
            }
            else if (!Input.IsActionJustPressed("move_up"))
            {
                controller.AnimatedSprite.Play("climbing");
            }
            controller.Player.Velocity = new Vector2(controller.VelocityIntoWall * controller.JumpVelocity, 
                verticalDirection * controller.WallClimbSpeed);
            controller.Player.MoveAndSlide();
            if (!controller.Player.IsOnWall())
            {
                controller.Player.Velocity = new Vector2(controller.VelocityIntoWall * controller.Speed * 0.5f, controller.Player.Velocity.Y);
                controller.Player.MoveAndSlide();
                controller.ChangeState(new IdleState(controller));
            }
        }
        else if (Input.IsActionPressed("move_down"))
        {
            controller.AnimatedSprite.Play("climbing");
            controller.Player.Velocity = new Vector2(controller.VelocityIntoWall * controller.JumpVelocity, 
                verticalDirection * controller.WallClimbSpeed);
            controller.Player.MoveAndSlide();
            if (!controller.Player.IsOnWall())
            {
                controller.Player.Velocity = new Vector2(0, controller.Player.Velocity.Y);
                controller.Player.MoveAndSlide();
                controller.ChangeState(new IdleState(controller));
            }
        }
        else if (!Input.IsActionPressed("grab_wall"))
        {
            controller.CoyoteTimeFromWallLeft = controller.CoyoteTimeLimit;
            if ((controller.Player.GetWallNormal().X * direction) < 0)
            {
                controller.ChangeState(new WallSlidingState(controller));
            }
            else
            {
                controller.ChangeState(new FallingState(controller));
            }
        }
        else
        {
            controller.Player.Velocity = new Vector2(controller.VelocityIntoWall * controller.JumpVelocity, 0);
            controller.Player.MoveAndSlide();

            if (!controller.Player.IsOnWall())
            {
                controller.Player.Velocity = new Vector2(direction * controller.JumpHorizontalSpeed, controller.Player.Velocity.Y);
                controller.Player.MoveAndSlide();
                controller.ChangeState(new FallingState(controller));
            }
        }
    }
}