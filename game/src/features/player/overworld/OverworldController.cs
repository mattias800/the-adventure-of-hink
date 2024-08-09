using Godot;
using System;

public partial class OverworldController : Node2D
{
    [Export]
    public CharacterBody2D Player;
    
    [Export]
    public AnimatedSprite2D AnimatedSprite;

    public bool Enabled;
    
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
    
    public override void _PhysicsProcess(double delta)
    {
        if (!Enabled)
        {
            return;
        }
        
        var direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        switch (_state)
        {
            case State.Disabled:
                break;

            case State.Idle:
                if (direction.Length() > 0.0)
                {
                    AnimatedSprite.Play("walk");
                }
                else
                {
                    AnimatedSprite.Play("idle");
                }

                if (direction.Length() > 0.0)
                {
                    Player.Velocity = Player.Velocity.Lerp(direction * _speed, 0.3f);
                }
                else
                {
                    // Slide when stopping
                    Player.Velocity = new Vector2(
                        Mathf.MoveToward(Player.Velocity.X, 0, _speed),
                        Mathf.MoveToward(Player.Velocity.Y, 0, _speed)
                    );
                }

                Player.MoveAndSlide();
                break;
        }

        if (Player.Velocity.X > 0)
        {
            AnimatedSprite.FlipH = false;
            if (_playerDirection == PlayerDirection.Left)
            {
                // Emit signal: player_turned("right")
            }
            _playerDirection = PlayerDirection.Right;
        }
        else if (Player.Velocity.X < 0)
        {
            AnimatedSprite.FlipH = true;
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
                AnimatedSprite.Play("idle");
                break;

            case State.Idle:
                AnimatedSprite.Play("idle");
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
        Enabled = true;
        EnterState(State.Idle);
    }

    public void Disable()
    {
        Enabled = false;
        EnterState(State.Disabled);
    }
}