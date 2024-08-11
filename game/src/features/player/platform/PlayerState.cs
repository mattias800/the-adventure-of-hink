using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform;

public abstract class PlayerState
{
    public string Name;
    protected PlatformController Controller;

    public PlayerState(string name, PlatformController controller)
    {
        Controller = controller;
        Name = name;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void PhysicsProcess(double delta, PlayerSkillsState playerSkillsState);
}