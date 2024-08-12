using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;
using Theadventureofhink.utils;

public partial class Collectible : Area2D
{
    [Export] public CollectibleInstance Instance;

    private AudioStreamPlayer2D _sound;
    
    private GameState _gameState;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>(Singletons.GameState);
        _sound = GetNode<AudioStreamPlayer2D>("Sound");
        
        if (_gameState.CollectiblesState.HasBeenCollected(Instance))
        {
            QueueFree();
        }
    }

    public void OnBodyEntered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _gameState.CollectiblesState.SetCollected(Instance);
            Visible = false;
            _sound.Play();
        }
    }
}