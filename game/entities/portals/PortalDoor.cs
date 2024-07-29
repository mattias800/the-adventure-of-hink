using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.portals;

public partial class PortalDoor : Node2D, IPortal
{
    [Export(PropertyHint.File, "*.tscn")] public string NextScenePath;

    [Export] public string TargetPortalName;

    [Signal]
    public delegate void PlayerEnteredPortalEventHandler();

    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
    }

    public void OnPlayerEnteredDoor()
    {
        if (NextScenePath != null)
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