using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.fire;
using Theadventureofhink.game_state;

public partial class Level01 : Node2D
{
    private CutsceneManager _cutsceneManager;
    private MusicManager _musicManager;
    private GameStateManager _gameStateManager;
    private Resource resource;

    private Firepit _firepit;


    public override void _Ready()
    {
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _musicManager = GetNode<MusicManager>(Singletons.MusicManager);
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
        _firepit = GetNode<Firepit>("Rooms/Room1/Firepit");
        resource = GD.Load("res://world/world_01/center/hometown-western-forest/level_01.dialogue");

        _musicManager.PlayTrack(Tracks.Track.EarlyMorning);

        if (!_gameStateManager.GameState.WorldState.HometownWesternForestState.HasEverVisitedRoom1.Value)
        {
            _firepit.State = FireState.OnFire;
        }
        else
        {
            _firepit.State = FireState.JustSmoke;
        }
    }

    public async void OnPlayerEnteredRoom(string nextRoom, string previousRoom)
    {
        if (nextRoom == "Room1")
        {
            GD.Print("OnPlayerEnteredRoom1");
            if (GameStateManager.Once(_gameStateManager.GameState.WorldState.HometownWesternForestState.HasEverVisitedRoom1))
            {
                await _cutsceneManager.PlayFullDialogue(resource, "room1_entry", 1.0f);
            }
        }
    }
}