using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class LittleMushroom : Node2D
{
    private AnimatedSprite2D _animatedSprite2D;
    private Talkable _talkable;

    private Resource _resource;

    private GameManager _gameManager;
    private GameState _gameState;
    private CutsceneManager _cutsceneManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _gameState = GetNode<GameState>(Singletons.GameState);
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _talkable = GetNode<Talkable>("Talkable");
        _resource = GD.Load("res://world/characters/little_mushroom/little_mushroom.dialogue");

        _animatedSprite2D.Play("idle");
    }

    public override void _Process(double delta)
    {
        if (_gameManager.Player.GlobalPosition < GlobalPosition)
        {
            _animatedSprite2D.FlipH = true;
        }
        else
        {
            _animatedSprite2D.FlipH = false;
        }
    }

    public async void OnTalk()
    {
        if (GameState.Once(_gameState.CharactersState.LittleMushroomState.HasEverMet))
        {
            await _cutsceneManager.StartTimeline(_resource, "start");
        }
        else
        {
            await _cutsceneManager.StartTimeline(_resource, "second");
        }

        _talkable.Activate();

    }
}