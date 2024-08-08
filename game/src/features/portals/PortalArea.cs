using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.portals;
using Theadventureofhink.utils;
using Theadventureofhink.world;

public partial class PortalArea : Area2D, IPortal
{
    [Export] public Stage NextStage;

    [Export] public string TargetPortalName;

    [Signal]
    public delegate void PlayerEnteredPortalEventHandler();

    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
    }

    public void OnBodyEntered(Node2D body)
    {
        GD.Print("OnBodyEntered portal");

        if (CollisionUtil.IsPlayer(body))
        {
            GD.Print("Entered portal: " + Name);
            _gameManager.OnPlayerEnteredPortal(this);
            EmitSignal(SignalName.PlayerEnteredPortal);
        }
    }

    public Stage GetNextStage()
    {
        return NextStage;
    }

    public string GetTargetPortalName()
    {
        return TargetPortalName;
    }

    public string GetName()
    {
        return Name;
    }

    public Vector2 GetSpawnPosition()
    {
        return GlobalPosition;
    }
}