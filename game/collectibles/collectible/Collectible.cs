using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;
using Theadventureofhink.utils;

namespace Theadventureofhink.collectibles.collectible;

public partial class Collectible : Area2D
{
    [Signal]
    public delegate void CollectedEventHandler();

    [Export] public CollectibleInstance Instance;

    private AudioStreamPlayer2D _sound;

    private GameStateManager _gameStateManager;

    public override void _Ready()
    {
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
        _sound = GetNode<AudioStreamPlayer2D>("Sound");

        if (_gameStateManager.GameState.CollectiblesState.HasBeenCollected(Instance))
        {
            QueueFree();
        }
    }

    public new void OnBodyEntered(Node2D body)
    {
        var collected = _gameStateManager.GameState.CollectiblesState.HasBeenCollected(Instance);
        
        if (!collected && CollisionUtil.IsPlayer(body))
        {
            _gameStateManager.GameState.CollectiblesState.SetCollected(Instance);
            Visible = false;
            _sound.Play();
            EmitSignal(SignalName.Collected);
        }
    }
}