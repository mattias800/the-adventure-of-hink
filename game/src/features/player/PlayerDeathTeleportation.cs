using Godot;
using System;

public partial class PlayerDeathTeleportation : Node2D
{
    private AnimatedSprite2D _animatedSprite;

    public enum State
    {
        Disabled,
        Teleporting,
        Appearing
    }

    private State _state = State.Disabled;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _state = State.Disabled;
        _animatedSprite.Visible = false;
        
        // Teleportation animation is looped, so this is triggered only by player appear animation.
        _animatedSprite.AnimationFinished += OnPlayerAppearDone;
    }

    public void PlayTeleporting()
    {
        GD.Print("play_teleporting");
        _state = State.Teleporting;
        _animatedSprite.Visible = true;
        _animatedSprite.Play("teleporting");
    }

    public void PlayPlayerAppearing()
    {
        _state = State.Appearing;
        _animatedSprite.Visible = true;
        _animatedSprite.Play("player_appearing");
    }

    private void OnPlayerAppearDone()
    {
        _animatedSprite.Visible = false;
        _state = State.Disabled;
    }
}