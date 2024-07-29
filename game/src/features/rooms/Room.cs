using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

public partial class Room : Area2D
{
    [Export] public bool Enabled = true;

    public CollisionShape2D CollisionShape;

    [Signal]
    public delegate void PlayerEnteredRoomEventHandler();

    [Signal]
    public delegate void PlayerExitedRoomEventHandler();
    
    private int _framesLeftInitialPlayerChecked;
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _framesLeftInitialPlayerChecked = 10;
    }

    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_framesLeftInitialPlayerChecked > 0)
        {
            _framesLeftInitialPlayerChecked -= 1;
            var roomDetection = _gameManager.Player.GetNode<Area2D>("RoomDetection");
            if (Enabled && roomDetection.OverlapsArea(this))
            {
                EmitSignal(SignalName.PlayerEnteredRoom);
            }
        }
    }

    public void OnAreaEntered(Area2D area)
    {
        if (Enabled && CollisionUtil.IsPlayer(area.GetParent()))
        {
            GD.Print("Player entered room: " + Name);
            EmitSignal(SignalName.PlayerEnteredRoom);
        }
    }

    public void OnAreaExited(Area2D area)
    {
        if (Enabled && CollisionUtil.IsPlayer(area.GetParent()))
        {
            GD.Print("Player exited room: " + Name);
            EmitSignal(SignalName.PlayerExitedRoom);
        }
    }
}