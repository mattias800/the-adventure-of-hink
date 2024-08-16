using Godot;
using System;
using System.Collections.Generic;
using Theadventureofhink.autoloads;
using Theadventureofhink.world;

public partial class MainMenu : Node2D
{
    private bool _enabled;

    private GameManager _gameManager;
    private Pointer _pointer;

    private readonly List<string> _controlNames =
    [
        "Continue",
        "NewGame",
        "LoadGame",
        "Settings"
    ];

    private int _selectedControlIndex;

    private bool _waiting = true;
    private float _timeUntilInputAllowed;


    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _pointer = GetNode<Pointer>("Pointer");
    }

    public void Enable()
    {
        GD.Print("Enable main menu");
        _enabled = true;
        _timeUntilInputAllowed = 0.1f;
        _selectedControlIndex = 0;
        MovePointerToSelected();
        _pointer.Visible = true;
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

        var readyForInput = _enabled && _waiting && _timeUntilInputAllowed <= 0.0f;

        if (readyForInput)
        {
            if (Input.IsActionJustPressed("jump"))
            {
                _waiting = false;
                _gameManager.LoadNextStage(Stage.HometownWesternForest, "IntroPortal");
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