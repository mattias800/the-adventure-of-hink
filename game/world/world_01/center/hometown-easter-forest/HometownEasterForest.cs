using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class HometownEasterForest : Node2D
{
    private Resource _resource =
        ResourceLoader.Load("res://world/world_01/center/hometown-easter-forest/bats.dialogue");

    private GameState _gameState;

    private bool _hasSeenDialogue;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>(Singletons.GameState);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _gameState.PlayerState.PlayerSkillsState.CanClimbWalls.SetValue(true);
    }

    public async void OnPlayerEnterRoom(string nextRoomName, string previousRoomName)
    {
        GD.Print("OnPlayerEnterRoom");
        GD.Print(nextRoomName);
        GD.Print(previousRoomName);
        
        if (nextRoomName == "Room4" && !_hasSeenDialogue)
        {
            await _cutsceneManager.PlayFullDialogue(_resource, "start");
            _hasSeenDialogue = true;
        }
    }
}