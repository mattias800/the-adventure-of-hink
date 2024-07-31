using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class DialogueStateHelper : Node2D
{
    private GameState _gameState;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>(Singletons.GameState);
    }

    public bool PlayerCanDoubleJump()
    {
        return _gameState.PlayerState.PlayerSkillsState.CanDoubleJump.Value();
    }

    public bool PlayerCanWallJump()
    {
        return _gameState.PlayerState.PlayerSkillsState.CanWallJump.Value();
    }

    public bool PlayerCanDash()
    {
        return _gameState.PlayerState.PlayerSkillsState.CanDash.Value();
    }
    
    public bool PlayerCanClimbWalls()
    {
        return _gameState.PlayerState.PlayerSkillsState.CanClimbWalls.Value();
    }
}