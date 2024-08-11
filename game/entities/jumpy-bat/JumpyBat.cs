using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

public partial class JumpyBat : Area2D
{
    private AnimatedSprite2D _animatedSprite2D;

    private bool active = true;

    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        _animatedSprite2D.Play("idle");
    }

    public void OnBodyShapeEntered(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index)
    {
        if (active && CollisionUtil.IsPlayer(body))
        {
            _gameManager.RespawnPlayer();
        }
    }

    public async void OnAreaShapeEntered(Rid area_rid, Area2D area, int area_shape_index, int local_shape_index)
    {
        if (area.IsInGroup("player_bounce") && active)
        {
            if (_animatedSprite2D.Animation == "hit")
            {
                _animatedSprite2D.Frame = 0;
            }
            else
            {
                _animatedSprite2D.Play("hit");
                _animatedSprite2D.AnimationLooped += OnHitAnimationDone;
            }
            
            if (area.GlobalPosition.Y > GlobalPosition.Y)
            {
                _gameManager.RespawnPlayer();
            }
            else
            {
                _gameManager.Player.OnHitJumpSource();
                active = false;
                await ToSignal(GetTree().CreateTimer(0.1), "timeout");
                active = true;
            }
        }
    }


    private void OnHitAnimationDone()
    {
        _animatedSprite2D.Play("idle");
        _animatedSprite2D.AnimationFinished -= OnHitAnimationDone;
    }
}