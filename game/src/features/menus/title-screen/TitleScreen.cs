using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.features.menus.load_game_menu;
using Theadventureofhink.world;

public partial class TitleScreen : Node2D
{
    private GameManager _gameManager;
    private Theadventureofhink.features.menus.main_menu.MainMenu _mainMenu;
    private LoadGameMenu _loadGameMenu;
    private Label _pressAnyKey;
    private Label _title;

    private bool _waiting = true;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _mainMenu = GetNode<Theadventureofhink.features.menus.main_menu.MainMenu>("MainMenu");
        _loadGameMenu = GetNode<LoadGameMenu>("LoadGameMenu");

        _pressAnyKey = GetNode<Label>("PressAnyKey");
        _title = GetNode<Label>("Title");

        _mainMenu.Visible = false;
        _loadGameMenu.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (_waiting && Input.IsActionJustPressed("ui_cancel"))
        {
            Callable.From(() => GetTree().Quit()).CallDeferred();
        }

        if (_waiting && Input.IsActionJustPressed("jump"))
        {
            _pressAnyKey.Visible = false;
            GetViewport().SetInputAsHandled();
            _waiting = false;

            _mainMenu.Visible = true;
            _mainMenu.Enable();
        }
    }

    public void OnSelectLoadGameMenu()
    {
        _mainMenu.Visible = false;
        _mainMenu.Disable();
        _title.Visible = false;
        _loadGameMenu.Visible = true;
        _loadGameMenu.Enable();
    }

    public void OnLoadGameMenuExited()
    {
        _loadGameMenu.Visible = false;
        _loadGameMenu.Disable();
        _title.Visible = true;
        _mainMenu.Visible = true;
        _mainMenu.Enable();
    }
}