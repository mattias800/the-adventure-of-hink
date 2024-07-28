public class DisabledState : PlayerState
{
    public DisabledState(PlatformController controller) : base(controller, "Disabled")
    {
    }
    
    public override void Enter()
    {
        // Disable player physics processing and animations
        controller.Player.SetPhysicsProcess(false);
        controller.AnimatedSprite.Stop();
    }

    public override void Exit()
    {
        // Re-enable player physics processing and animations
        controller.Player.SetPhysicsProcess(true);
        controller.AnimatedSprite.Play();
    }

    public override void PhysicsProcess(double delta)
    {
        // Do nothing as the player is in a disabled state
    }
}