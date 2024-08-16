using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class DialogueStateHelper : Node2D
{
    private GameStateManager _gameStateManager;

    public override void _Ready()
    {
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
    }

    public bool PlayerCanDoubleJump()
    {
        return _gameStateManager.GameState.PlayerState.PlayerSkillsState.CanDoubleJump.Value;
    }

    public bool PlayerCanWallJump()
    {
        return _gameStateManager.GameState.PlayerState.PlayerSkillsState.CanWallJump.Value;
    }

    public bool PlayerCanDash()
    {
        return _gameStateManager.GameState.PlayerState.PlayerSkillsState.CanDash.Value;
    }
    
    public bool PlayerCanClimbWalls()
    {
        return _gameStateManager.GameState.PlayerState.PlayerSkillsState.CanClimbWalls.Value;
    }
    public bool PlayerHasBlacksmithsHammer()
    {
        return _gameStateManager.GameState.PlayerState.PlayerItemsState.GotBlacksmithsHammers.Value;
    }
}