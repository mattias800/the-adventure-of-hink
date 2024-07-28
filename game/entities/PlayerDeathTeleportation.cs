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
    }

    public override void _Process(double delta)
    {
        switch (_state)
        {
            case State.Disabled:
                // animatedSprite.Visible = false;
                return;
            case State.Teleporting:
                _animatedSprite.Play("teleporting");
                break;
            case State.Appearing:
                return;
        }
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
        _animatedSprite.AnimationFinished += OnPlayerAppearDone;
    }

    private void OnPlayerAppearDone()
    {
        _animatedSprite.Visible = false;
        _state = State.Disabled;
    }
}