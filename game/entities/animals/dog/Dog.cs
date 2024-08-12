using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class Dog : Node2D
{
    [Export] public bool Flipped;
    
    private Resource _resource;
    private CutsceneManager _cutsceneManager;
    private AnimatedSprite2D _animatedSprite2D;
    private Talkable _talkable;
    private AudioStreamPlayer2D _barkSound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _cutsceneManager = GetNode<CutsceneManager>(Singletons.CutsceneManager);
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _talkable = GetNode<Talkable>("Talkable");
        _barkSound = GetNode<AudioStreamPlayer2D>("BarkSound");
        
        _resource = GD.Load("res://entities/animals/dog/dog.dialogue");
        
        _animatedSprite2D.Play("idle");
    }

    public override void _Process(double delta)
    {
        _animatedSprite2D.FlipH = Flipped;
    }

    public async void OnTalk()
    {
        _animatedSprite2D.Play("duck");
        _barkSound.Play();
        await _cutsceneManager.PlayFullDialogue(_resource, "start");
        _animatedSprite2D.Play("idle");
        _barkSound.Play();
        _talkable.Activate();
    }
}