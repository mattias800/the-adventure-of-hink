using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class TitleScreen : Node2D
{
    private GameManager _gameManager;

    private bool _waiting = true;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
    }

    public override void _Process(double delta)
    {
        if (_waiting && Input.IsActionJustPressed("jump"))
        {
            _waiting = false;
            _gameManager.LoadNextScene("res://world/world_01/level_01/level_01.tscn", "StartPortal");
        }
    }
}