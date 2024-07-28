public abstract class PlayerState
{
    protected PlatformController controller;
    public string Name;

    public PlayerState(PlatformController controller, string name)
    {
        this.controller = controller;
        this.Name = name;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void PhysicsProcess(double delta);

}