using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

public partial class Spikes : Sprite2D
{
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
    }

    public void OnArea2dBodyEntered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _gameManager.RespawnPlayer();
            // CallDeferred(nameof(_gameManager.RespawnPlayer));
        }
    }
}