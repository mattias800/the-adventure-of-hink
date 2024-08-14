using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class HometownFields : Node2D
{
    [Export] public Hammer Hammer;

    private GameState _gameState;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>(Singletons.GameState);
    }

    public override void _Process(double delta)
    {
    }

    public async void OnDialogueFinished()
    {
        if (!_gameState.PlayerState.PlayerItemsState.GotBlacksmithsHammers.Value())
        {
            await Hammer.CollectHammer();
        }
    }
}