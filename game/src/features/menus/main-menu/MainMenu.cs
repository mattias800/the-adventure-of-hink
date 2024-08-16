using System.Collections.Generic;
using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_saves;
using Theadventureofhink.world;

namespace Theadventureofhink.features.menus.main_menu;

public partial class MainMenu : Node2D
{
    [Signal]
    public delegate void LoadGameMenuSelectedEventHandler();

    private GameManager _gameManager;
    private GameSaveSlotManager _gameSaveSlotManager;
    private Pointer _pointer;

    private readonly List<string> _controlNames =
    [
        "Continue",
        "NewGame",
        "LoadGame",
        "Settings"
    ];

    private int _selectedControlIndex;

    private bool _enabled;
    private float _timeUntilInputAllowed;


    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _gameSaveSlotManager = GetNode<GameSaveSlotManager>(Singletons.GameSaveSlotManager);
        _pointer = GetNode<Pointer>("Pointer");
    }

    public void Enable()
    {
        GD.Print("Enable main menu");
        _timeUntilInputAllowed = 0.1f;
        _enabled = true;
        MovePointerToSelected();
        _pointer.Visible = true;
    }

    public void Disable()
    {
        _enabled = false;
        _pointer.Visible = false;
    }

    private void MovePointerToSelected()
    {
        var control = GetNode<Control>("VBoxContainer/" + _controlNames[_selectedControlIndex]);
        GD.Print("moving pointer to " + control.Name);
        _pointer.PointAtAndPlaySound(control);
    }

    public override void _Process(double delta)
    {
        if (_timeUntilInputAllowed > 0.0f)
        {
            _timeUntilInputAllowed -= (float)delta;
        }

        var readyForInput = _enabled && _timeUntilInputAllowed <= 0.0f;

        if (readyForInput)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                Callable.From(() => GetTree().Quit()).CallDeferred();
            }

            if (Input.IsActionJustPressed("ui_accept"))
            {
                if (_selectedControlIndex == 0)
                {
                    _gameSaveSlotManager.CurrentSlotIndex = 0;
                    _gameSaveSlotManager.LoadGameFromCurrentSlot();
                    _gameManager.LoadNextStageStoredInGameState();
                }

                if (_selectedControlIndex == 1)
                {
                    _gameSaveSlotManager.CurrentSlotIndex = 0;
                    _gameManager.LoadNextStageAndSave(Stage.HometownWesternForest, "IntroPortal");
                }

                if (_selectedControlIndex == 2)
                {
                    EmitSignal(SignalName.LoadGameMenuSelected);
                }
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
}