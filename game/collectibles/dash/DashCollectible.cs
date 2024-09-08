using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;
using Theadventureofhink.utils;

public partial class DashCollectible : Node2D
{
    private Area2D _area2d;
    private GameStateManager _gameStateManager;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _area2d = GetNode<Area2D>("Area2D");
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);

        if (_gameStateManager.GameState.PlayerState.PlayerSkillsState.CanDash.Value)
        {
            QueueFree();
        }
    }

    public override void _Process(double delta)
    {
    }

    public async void OnBodyEntered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            await _cutsceneManager.PlaySingleCharacterLine("Hink", "Holy shit!");
            await _cutsceneManager.PlaySingleCharacterLine("Hink", "The dash orb! I found it!");
            await _cutsceneManager.PlaySingleCharacterLine("Hink", "Maybe this thing can get me out of here.");
            _gameStateManager.GameState.PlayerState.PlayerSkillsState.CanDash.SetValue(true);
            QueueFree();
        }
    }
}