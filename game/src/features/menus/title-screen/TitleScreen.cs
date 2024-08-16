using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.world;

public partial class TitleScreen : Node2D
{
    private GameManager _gameManager;
    private MainMenu _mainMenu;
    private Label _pressAnyKey;

    private bool _waiting = true;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _mainMenu = GetNode<MainMenu>("MainMenu");
        _pressAnyKey = GetNode<Label>("PressAnyKey");
        
        _mainMenu.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (_waiting && Input.IsActionJustPressed("jump"))
        {
            _pressAnyKey.Visible = false;
            GetViewport().SetInputAsHandled();
            _waiting = false;

            _mainMenu.Visible = true;
            _mainMenu.Enable();
        }
    }
}