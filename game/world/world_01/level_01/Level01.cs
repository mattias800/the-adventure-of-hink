using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.fire;
using Theadventureofhink.game_state;

public partial class Level01 : Node2D
{
    private CutsceneManager _cutsceneManager;
    private Theadventureofhink.autoloads.MusicManager _musicManager;
    private GameState _gameState;
    private Resource resource;

    private Firepit _firepit;


    public override void _Ready()
    {
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _musicManager = GetNode<Theadventureofhink.autoloads.MusicManager>(Singletons.MusicManager);
        _gameState = GetNode<GameState>(Singletons.GameState);
        _firepit = GetNode<Firepit>("Rooms/Room1/Firepit");
        resource = GD.Load("res://world/world_01/level_01/level_01.dialogue");

        _musicManager.PlayTrack(Tracks.Track.EarlyMorning);

        if (!_gameState.WorldState.Level01State.HasEverVisitedRoom1.Value())
        {
            _firepit.State = FireState.OnFire;
        }
        else
        {
            _firepit.State = FireState.JustSmoke;
        }
    }

    public async void OnPlayerEnteredRoom1()
    {
        if (GameState.Once(_gameState.WorldState.Level01State.HasEverVisitedRoom1))
        {
            await _cutsceneManager.StartTimeline(resource, "room1_entry", 1.0f);
        }
    }
}