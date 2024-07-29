using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.portals;
using Theadventureofhink.utils;

public partial class PortalArea : Area2D, IPortal
{
    [Export(PropertyHint.File, "*.tscn")]
    public string NextScenePath;
    
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
        if (CollisionUtil.IsPlayer(body) && NextScenePath != null)
        {
            GD.Print("Entered portal: " + Name);
            _gameManager.OnPlayerEnteredPortal(this);
            EmitSignal(SignalName.PlayerEnteredPortal);
        }
    }

    public string? GetNextScenePath()
    {
        return NextScenePath;
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