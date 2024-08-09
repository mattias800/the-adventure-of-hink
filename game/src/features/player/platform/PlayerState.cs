using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform;

public abstract class PlayerState
{
    protected PlatformController Controller;
    public string Name;

    public PlayerState(PlatformController controller, string name)
    {
        this.Controller = controller;
        this.Name = name;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState);
}