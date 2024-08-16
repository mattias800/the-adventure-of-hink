using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class Overworld01 : Node2D
{
    private Resource resource = ResourceLoader.Load("res://world/overworld_01/overworld_01.dialogue");

    private MusicManager _musicManager;
    private GameStateManager _gameStateManager;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _musicManager = GetNode<MusicManager>(Singletons.MusicManager);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);

        _musicManager.PlayTrack(Tracks.Track.WhisperingShadows);
    }

    public async void OnPlayerEnterScene()
    {
        if (GameStateManager.Once(_gameStateManager.GameState.WorldState.Overworld01State.HasEverVisited))
        {
            await _cutsceneManager.PlayFullDialogue(resource, "first_entry", 1.0f);
        }
    }
}