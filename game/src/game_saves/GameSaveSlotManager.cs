using System;
using System.Collections.Generic;
using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

namespace Theadventureofhink.game_saves;

public partial class GameSaveSlotManager : Node
{
    public int CurrentSlotIndex = -1;

    public List<GameSaveSlot> Slots =
    [
        new()
        {
            Index = 0,
            FileName = "Save00"
        },
        new()
        {
            Index = 1,
            FileName = "Save01"
        },
        new()
        {
            Index = 2,
            FileName = "Save02"
        }
    ];

    private GameStateManager _gameStateManager;

    public override void _Ready()
    {
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
    }

    public void LoadGameFromCurrentSlot()
    {
        if (CurrentSlotIndex is >= 0 and <= 2)
        {
            _gameStateManager.GameState = GameSaveFileReader.LoadFileToGameState(Slots[CurrentSlotIndex].FileName);
        }
    }

    public void SaveGameToCurrentSlot()
    {
        if (CurrentSlotIndex is >= 0 and <= 2)
        {
            _gameStateManager.GameState.PlayerState.PlayerStatsState.LastSaveDateTimeIso =
                FormatIsoDateString(new DateTime());
            
            GameSaveFileReader.SaveGameStateToFile(_gameStateManager.GameState, Slots[CurrentSlotIndex].FileName);
        }
    }

    public string FormatIsoDateString(DateTime dateTime)
    {
        return dateTime.ToString("o");
    }
}