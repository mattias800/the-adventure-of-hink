using System.Globalization;
using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_saves;
using Theadventureofhink.utils;
using Theadventureofhink.world;

namespace Theadventureofhink.features.menus.load_game_menu;

public partial class SaveSlotPanel : Node2D
{
    private GameSaveSlotManager _gameSaveSlotManager;
    private Label _slotIndex;
    private Label _levelName;
    private Label _playtime;

    public override void _Ready()
    {
        _gameSaveSlotManager = GetNode<GameSaveSlotManager>(Singletons.GameSaveSlotManager);
        _slotIndex = GetNode<Label>("VBoxContainer/HBoxContainer/Left/SlotIndex");
        _levelName = GetNode<Label>("VBoxContainer/HBoxContainer/Right/LevelName");
        _playtime = GetNode<Label>("VBoxContainer/HBoxContainer/Right/Playtime");
    }

    public void PopulateWithSaveGame(int slotIndex)
    {
        _slotIndex.Text = slotIndex + 1 + ".";
        var gameState = _gameSaveSlotManager.ReadGameStateForSlot(slotIndex);
        GD.Print("read save " + slotIndex);
        GD.Print(gameState);
        if (gameState != null)
        {
            _levelName.Text = Stages.GetStateInfo((Stage)gameState.PlayerPositionState.LastStage).Name;
            _playtime.Text =
                "Playtime: " +
                DurationFormatter.FormatSecondsAsPlayTime((int)gameState.PlayerState.PlayerStatsState.SecondsPlayed);
        }
        else
        {
            _levelName.Text = "No data";
            _playtime.Text = "";
        }
    }
}