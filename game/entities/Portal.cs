using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

public partial class Portal : Area2D
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

}