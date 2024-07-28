using Godot;
using Theadventureofhink.utils;

public partial class Talkable : Area2D
{
    [Signal]
    public delegate void TalkStartedEventHandler();

    private Sprite2D _speechBubble;

    private bool _playerIsInside;
    private bool _active = true;

    public override void _Ready()
    {
        _speechBubble = GetNode<Sprite2D>("SpeechBubble");
    }

    public override void _Process(double delta)
    {
        if (_playerIsInside && _active)
        {
            _speechBubble.Scale = Lerper.Lerp(_speechBubble.Scale, Vector2.One, 0.1f);
        }
        else
        {
            _speechBubble.Scale = Lerper.Lerp(_speechBubble.Scale, Vector2.Zero, 0.1f);
        }

        if (_playerIsInside && Input.IsActionJustPressed("interact") && _active)
        {
            Talk();
        }
    }

    public void Talk()
    {
        _active = false;
        EmitSignal(SignalName.TalkStarted);
    }

    public void Activate()
    {
        _active = true;
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