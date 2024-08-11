using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform.states;

public class DisabledState(PlatformController controller) : PlayerState("Disabled", controller)
{
    public override void Enter()
    {
        // Disable player physics processing and animations
        Controller.Player.SetPhysicsProcess(false);
        Controller.AnimatedSprite.Stop();
    }

    public override void Exit()
    {
        // Re-enable player physics processing and animations
        Controller.Player.SetPhysicsProcess(true);
        Controller.AnimatedSprite.Play();
    }

    public override void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState)
    {
        // Do nothing as the player is in a disabled state
    }
}