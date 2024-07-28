using Godot;
using System.Linq;
using Theadventureofhink.autoloads;

public partial class CameraManager : Node
{
    public Camera Camera;

    private GameManager _gameManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var room = GetRoomContainingPlayer();
        if (room != null)
        {
            CameraLimiter.ApplyCollisionShapeToCameraLimits(Camera, room.CollisionShape);
        }
        else
        {
            Camera.ClearCameraLimits();
        }
    }


    public void SetCameraTarget(Node2D target)
    {
        Camera.SetCameraTarget(target);
    }

    public void SetCamera(Camera c)
    {
        Camera = c;
    }

    public Room? GetRoomContainingPlayer()
    {
        var rooms = GetTree().GetNodesInGroup("rooms").OfType<Room>();

        foreach (var room in rooms)
        {
            if (room.CollisionShape == null)
            {
                GD.Print("Missing collision shape on room node.");
                continue;
            }

            var roomDetection = _gameManager.Player.GetNode<Area2D>("RoomDetection");
            if (roomDetection == null)
            {
                GD.Print("_gameManager.Player=" + _gameManager.Player.Name);
                GD.Print("CameraManager found no room_detection node on player.");
                GetTree().Quit();
            }
            if (roomDetection != null && room.Enabled && roomDetection.OverlapsArea(room))
            {
                return room;
            }
        }

        return null;
    }
}