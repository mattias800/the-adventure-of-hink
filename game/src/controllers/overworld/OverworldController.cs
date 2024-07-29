using Godot;
using System;

public partial class OverworldController : Node2D
{
    // Enums
    public enum State
    {
        Disabled,
        Idle
    }

    public enum PlayerDirection
    {
        Left,
        Right
    }

    // Constants
    private float _speed = 80.0f;

    // Variables
    private State _state = State.Idle;
    private PlayerDirection _playerDirection = PlayerDirection.Right;

    private CharacterBody2D _player;
    private AnimatedSprite2D _animatedSprite;

    public OverworldController(CharacterBody2D player, AnimatedSprite2D animatedSprite)
    {
        _player = player;
        _animatedSprite = animatedSprite;
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        switch (_state)
        {
            case State.Disabled:
                break;

            case State.Idle:
                if (direction.Length() > 0.0)
                {
                    _animatedSprite.Play("walk");
                }
                else
                {
                    _animatedSprite.Play("idle");
                }

                if (direction.Length() > 0.0)
                {
                    _player.Velocity = _player.Velocity.Lerp(direction * _speed, 0.3f);
                }
                else
                {
                    // Slide when stopping
                    _player.Velocity = new Vector2(
                        Mathf.MoveToward(_player.Velocity.X, 0, _speed),
                        Mathf.MoveToward(_player.Velocity.Y, 0, _speed)
                    );
                }

                _player.MoveAndSlide();
                break;
        }

        if (_player.Velocity.X > 0)
        {
            _animatedSprite.FlipH = false;
            if (_playerDirection == PlayerDirection.Left)
            {
                // Emit signal: player_turned("right")
            }
            _playerDirection = PlayerDirection.Right;
        }
        else if (_player.Velocity.X < 0)
        {
            _animatedSprite.FlipH = true;
            if (_playerDirection == PlayerDirection.Right)
            {
                // Emit signal: player_turned("left")
            }
            _playerDirection = PlayerDirection.Left;
        }
    }

    public void EnterState(State nextState)
    {
        GD.Print("Enter state: " + StateToString(nextState));
        switch (nextState)
        {
            case State.Disabled:
                _animatedSprite.Play("idle");
                break;

            case State.Idle:
                _animatedSprite.Play("idle");
                break;
        }
        _state = nextState;
    }

    public string StateToString(State s)
    {
        switch (s)
        {
            case State.Idle:
                return "IDLE";
            case State.Disabled:
                return "DISABLED";
            default:
                return "";
        }
    }

    public void Enable()
    {
        EnterState(State.Idle);
    }

    public void Disable()
    {
        EnterState(State.Disabled);
    }
}