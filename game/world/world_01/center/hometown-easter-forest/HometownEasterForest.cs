using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class HometownEasterForest : Node2D
{
    private Resource _resource =
        ResourceLoader.Load("res://world/world_01/center/hometown-easter-forest/bats.dialogue");

    private GameStateManager _gameStateManager;

    private bool _hasSeenBatSwarmDialogue;
    private bool _hasSeenSingleBatDialogue;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _gameStateManager.GameState.PlayerState.PlayerSkillsState.CanClimbWalls.SetValue(true);
    }

    public async void OnPlayerEnterRoom(string nextRoomName, string previousRoomName)
    {
        if (nextRoomName == "Room2" && !_hasSeenSingleBatDialogue)
        {
            await _cutsceneManager.PlaySingleCharacterLine("Batian", "Holy shit, who is that?");
            await _cutsceneManager.PlaySingleCharacterLine("Batian", "I hope he doesn't jump on me..");
            _hasSeenSingleBatDialogue = true;
        }
        if (nextRoomName == "Room4" && !_hasSeenBatSwarmDialogue)
        {
            await _cutsceneManager.PlayFullDialogue(_resource, "start");
            _hasSeenBatSwarmDialogue = true;
        }
    }
}