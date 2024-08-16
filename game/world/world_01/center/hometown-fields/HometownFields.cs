using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class HometownFields : Node2D
{
    [Export] public Hammer Hammer;

    private GameStateManager _gameStateManager;

    public override void _Ready()
    {
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
    }

    public override void _Process(double delta)
    {
    }

    public async void OnDialogueFinished()
    {
        if (!_gameStateManager.GameState.PlayerState.PlayerItemsState.GotBlacksmithsHammers.Value)
        {
            await Hammer.CollectHammer();
        }
    }
}