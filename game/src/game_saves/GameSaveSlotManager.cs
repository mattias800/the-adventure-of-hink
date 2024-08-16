#nullable enable
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

    public GameState? ReadGameStateForSlot(int slotIndex)
    {
        if (slotIndex is < 0 or > 2)
        {
            return null;
        }

        return GameSaveFileReader.LoadFileToGameState(Slots[slotIndex].FileName);
    }

    public void LoadGameFromCurrentSlot()
    {
        var gameState = ReadGameStateForSlot(CurrentSlotIndex);
        if (gameState != null)
        {
            _gameStateManager.GameState = gameState;
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