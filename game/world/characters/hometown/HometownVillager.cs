using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class HometownVillager : Node2D
{
    [Export] public string Dialogue;
    [Export] public AnimatedSprite2D AnimatedSprite2D;
    [Export] public bool SpriteTurnedLeft;
    [Export] public bool StartTurnedLeft;

    private Resource _resource;
    private Talkable _talkable;
    private CutsceneManager _cutsceneManager;
    private GameManager _gameManager;
    
    // State
    private bool _turnedLeft;

    public override void _Ready()
    {
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _talkable = GetNode<Talkable>("Talkable");
        _resource = GD.Load("res://world/characters/hometown/hometown_villager.dialogue");

        if (string.IsNullOrEmpty(Dialogue))
        {
            GD.PrintErr("Villager has no dialogue starting point set.");
        }
        if (AnimatedSprite2D == null)
        {
            GD.PrintErr("Villager animated sprite field is not set.");
        }
        if (_resource == null)
        {
            GD.PrintErr("Dialog .resource file is null for " + Name);
        }
        
        AnimatedSprite2D?.Play("idle");

        _turnedLeft = StartTurnedLeft;
    }

    public override void _Process(double delta)
    {
        if (_turnedLeft)
        {
            TurnLeft();
        }
        else
        {
            TurnRight();
        }

    }

    public async void OnTalk()
    {
        if (!string.IsNullOrEmpty(Dialogue))
        {
            TurnTowardsPlayer();
            await _cutsceneManager.StartTimeline(_resource, Dialogue);
            _talkable.Activate();
        }
        else
        {
            _talkable.Activate();
        }
    }

    public void TurnLeft()
    {
        AnimatedSprite2D.FlipH = !SpriteTurnedLeft;
    }

    public void TurnRight()
    {
        AnimatedSprite2D.FlipH = SpriteTurnedLeft;
    }

    public void TurnTowardsPlayer()
    {
        _turnedLeft = _gameManager.Player.GlobalPosition.X < GlobalPosition.X;
    }
}