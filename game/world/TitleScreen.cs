using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.world;

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
            _gameManager.LoadNextStage(Stage.HometownWesternForest, "IntroPortal");
        }
    }
}