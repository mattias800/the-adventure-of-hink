using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class HometownEasterForest : Node2D
{
    private GameState _gameState;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>(Singletons.GameState);
        _gameState.PlayerState.PlayerSkillsState.CanClimbWalls.SetValue(true);
    }
}