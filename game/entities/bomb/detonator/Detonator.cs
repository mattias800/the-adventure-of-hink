using Godot;
using System;

public partial class Detonator : Node2D
{
    [Signal]
    public delegate void DetonatedEventHandler();

    [Signal]
    public delegate void PlayerInteractedEventHandler();

    [Export] public bool DetonateOnInteract;
    [Export] public Bomb? Bomb;

    public enum DetonatorState
    {
        Idle,
        Arming,
        Detonated
    }

    private DetonatorState _state;
    private float _armTimeLeft;
    private AnimatedSprite2D _sprite;
    private AudioStreamPlayer2D _beepSound;
    private AudioStreamPlayer2D _toggleSound;

    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _beepSound = GetNode<AudioStreamPlayer2D>("BeepSound");
        _toggleSound = GetNode<AudioStreamPlayer2D>("ToggleSound");
        
        _state = DetonatorState.Idle;
        _sprite.Play("idle");
    }

    public override void _Process(double delta)
    {
        switch (_state)
        {
            case DetonatorState.Idle:
                break;
            case DetonatorState.Arming:
                var wasAbove1 = _armTimeLeft > 1.0f;
                
                _armTimeLeft -= (float)delta;
                if (_armTimeLeft <= 1.0f && wasAbove1)
                {
                    _beepSound.Play();
                }
                if (_armTimeLeft <= 0.0f)
                {
                    Detonate();
                }

                break;
            case DetonatorState.Detonated:
                break;
        }
    }

    public void Arm()
    {
        _armTimeLeft = 2.0f;
        _sprite.Play("arming");
        _state = DetonatorState.Arming;
        _toggleSound.Play();
        _beepSound.Play();
    }

    public void Detonate()
    {
        EmitSignal(SignalName.Detonated);
        _state = DetonatorState.Detonated;
        _sprite.Play("done");
        Bomb?.Detonate();
    }

    public void OnInteract()
    {
        EmitSignal(SignalName.PlayerInteracted);
    }
}