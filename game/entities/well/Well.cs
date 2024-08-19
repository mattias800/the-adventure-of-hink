using Godot;
using Theadventureofhink.utils;

public partial class Well : Node2D
{
    [Signal]
    public delegate void PlayerEnteredWellEventHandler();

    [Export] public bool IsOpen;

    private CollisionShape2D _collisionShape2D;

    public override void _Ready()
    {
        _collisionShape2D = GetNode<CollisionShape2D>("OpeningBody/CollisionShape2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        _collisionShape2D.Disabled = IsOpen;
    }

    public void OnArea2dBodyEntered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            EmitSignal(SignalName.PlayerEnteredWell);
        }
    }
}