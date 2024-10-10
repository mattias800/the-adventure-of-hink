using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

public partial class Checkpoint : Area2D
{
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
    }

    public new void OnBodyEntered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            GD.Print("Reached checkpoint.");
            _gameManager.SetCurrentCheckpointAndSave(this);
        }
    }
}