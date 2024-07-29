using Godot;
using System;
using Theadventureofhink.utils;

public partial class Interactible : Area2D
{
    [Signal]
    public delegate void PlayerInteractedEventHandler();

    [Export]
    public bool IsActive = true;

    private Sprite2D _speechBubble;

    private bool _playerIsInside;

    public override void _Ready()
    {
        _speechBubble = GetNode<Sprite2D>("SpeechBubble");
    }

    public override void _Process(double delta)
    {
        if (_playerIsInside && IsActive)
        {
            _speechBubble.Scale = Lerper.Lerp(_speechBubble.Scale, Vector2.One, 0.1f);
        }
        else
        {
            _speechBubble.Scale = Lerper.Lerp(_speechBubble.Scale, Vector2.Zero, 0.1f);
        }

        if (_playerIsInside && Input.IsActionJustPressed("interact") && IsActive)
        {
            Interact();
        }
    }

    public void Interact()
    {
        IsActive = false;
        EmitSignal(SignalName.PlayerInteracted);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void OnBodyEntered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _playerIsInside = true;
        }
    }


    public void OnBodyExited(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _playerIsInside = false;
        }
    }
}