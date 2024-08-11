using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

namespace Theadventureofhink.entities.jumpy_bat;

public partial class JumpyBat : Area2D
{
    private AnimatedSprite2D _animatedSprite2D;

    private bool _active = true;
    private float _timeUntilActive;

    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        _animatedSprite2D.Play("idle");
        // Idle animation is looped, this event will only get triggered by hit animation.
        _animatedSprite2D.AnimationFinished += OnHitAnimationDone;
    }

    public override void _Process(double delta)
    {
        if (_timeUntilActive > 0.0f)
        {
            _timeUntilActive -= (float)delta;
            if (_timeUntilActive <= 0)
            {
                _active = true;
            }
        }
    }

    public void OnBodyShapeEntered(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index)
    {
        if (_active && CollisionUtil.IsPlayer(body))
        {
            _gameManager.RespawnPlayer();
        }
    }

    public void OnAreaShapeEntered(Rid area_rid, Area2D area, int area_shape_index, int local_shape_index)
    {
        if (area.IsInGroup("player_bounce") && _active)
        {
            if (_animatedSprite2D.Animation == "hit")
            {
                _animatedSprite2D.Frame = 0;
            }
            else
            {
                _animatedSprite2D.Play("hit");
            }
            
            if (area.GlobalPosition.Y > GlobalPosition.Y)
            {
                Callable.From(() => _gameManager.RespawnPlayer()).CallDeferred();
            }
            else
            {
                _gameManager.Player.OnHitJumpSource();
                _active = false;
                _timeUntilActive = 0.1f;
            }
        }
    }


    private void OnHitAnimationDone()
    {
        _animatedSprite2D.Play("idle");
    }
}