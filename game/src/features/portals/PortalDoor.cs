using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.portals;
using Theadventureofhink.world;

public partial class PortalDoor : Node2D, IPortal
{
    [Export] public bool Enabled = true;
    [Export] public bool SoundEnabled = true;
    
    [Export] public Stage NextStage;

    [Export] public string TargetPortalName;

    [Signal]
    public delegate void PlayerEnteredPortalEventHandler();

    private GameManager _gameManager;
    private AudioStreamPlayer2D _openDoorSound;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _openDoorSound = GetNode<AudioStreamPlayer2D>("OpenDoorSound");
    }

    public void OnPlayerEnteredDoor()
    {
        if (Enabled)
        {
            GD.Print("Entered portal: " + Name);
            _openDoorSound.Play();
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

    public new string GetName()
    {
        return Name;
    }

    public Vector2 GetSpawnPosition()
    {
        return GlobalPosition;
    }
}