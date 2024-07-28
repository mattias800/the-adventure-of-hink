using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class Overworld01 : Node2D
{
    private Resource resource;

    private MusicManager _musicManager;
    private GameState _gameState;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _musicManager = GetNode<MusicManager>(Singletons.MusicManager);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _gameState = GetNode<GameState>(Singletons.GameState);
        resource = GD.Load("res://world/overworld_01/overworld_01.dialogue");

        _musicManager.PlayTrack(Tracks.Track.WhisperingShadows);
    }

    public async void OnPlayerEnterScene()
    {
        if (GameState.Once(_gameState.WorldState.Overworld01State.HasEverVisited))
        {
            await _cutsceneManager.StartTimeline(resource, "first_entry", 1.0f);
        }
    }
}