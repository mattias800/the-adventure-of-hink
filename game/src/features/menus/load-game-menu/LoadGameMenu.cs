using System.Collections.Generic;
using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_saves;

namespace Theadventureofhink.features.menus.load_game_menu;

public partial class LoadGameMenu : Node2D
{
    private GameManager _gameManager;
    private GameSaveSlotManager _gameSaveSlotManager;

    private Pointer _pointer;
    private SaveSlotPanel _saveSlotPanel0;
    private SaveSlotPanel _saveSlotPanel1;
    private SaveSlotPanel _saveSlotPanel2;

    private int _selectedControlIndex;

    private readonly List<string> _controlNames =
    [
        "SaveSlotPanel0",
        "SaveSlotPanel1",
        "SaveSlotPanel2"
    ];

    private bool _enabled;
    private bool _waiting = true;
    private float _timeUntilInputAllowed;


    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _gameSaveSlotManager = GetNode<GameSaveSlotManager>(Singletons.GameSaveSlotManager);

        _pointer = GetNode<Pointer>("Pointer");
        _saveSlotPanel0 = GetNode<SaveSlotPanel>("SaveSlotPanel0");
        _saveSlotPanel1 = GetNode<SaveSlotPanel>("SaveSlotPanel1");
        _saveSlotPanel2 = GetNode<SaveSlotPanel>("SaveSlotPanel2");

        _selectedControlIndex = 0;
        _enabled = false;

        _saveSlotPanel0.PopulateWithSaveGame(0);
        _saveSlotPanel1.PopulateWithSaveGame(1);
        _saveSlotPanel2.PopulateWithSaveGame(2);
    }

    public override void _Process(double delta)
    {
        if (_timeUntilInputAllowed > 0.0f)
        {
            _timeUntilInputAllowed -= (float)delta;
        }

        var readyForInput = _enabled && _waiting && _timeUntilInputAllowed <= 0.0f;

        if (readyForInput)
        {
            if (Input.IsActionJustPressed("jump"))
            {
                _waiting = false;
                _gameSaveSlotManager.CurrentSlotIndex = _selectedControlIndex;
                _gameSaveSlotManager.LoadGameFromCurrentSlot();
                _gameManager.LoadNextStageStoredInGameState();
            }

            if (Input.IsActionJustPressed("ui_up"))
            {
                if (_selectedControlIndex > 0)
                {
                    _selectedControlIndex--;
                    MovePointerToSelected();
                }
            }

            if (Input.IsActionJustPressed("ui_down"))
            {
                if (_selectedControlIndex <= 2)
                {
                    _selectedControlIndex++;
                    MovePointerToSelected();
                }
            }
        }
    }

    public void Enable()
    {
        GD.Print("Enable main menu");
        _enabled = true;
        _timeUntilInputAllowed = 0.1f;
        _selectedControlIndex = 0;
        MovePointerToSelected();
    }

    private void MovePointerToSelected()
    {
        var control = GetNode<Node2D>(_controlNames[_selectedControlIndex]);
        GD.Print("moving pointer to " + control.Name);
        _pointer.PointAtAndPlaySound(control.GlobalPosition + new Vector2(-8, 22));
    }
}