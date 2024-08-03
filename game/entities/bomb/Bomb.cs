using Godot;
using System;

public partial class Bomb : Node2D
{
    [Signal]
    public delegate void ExplodedEventHandler();

    [Export] public float FuseTime = 2.0f;

    private AnimatedSprite2D _animatedSprite2D;
    private AnimatedSprite2D _explosionAnimation;
    private CpuParticles2D _fire;
    private CpuParticles2D _smoke;
    private AudioStreamPlayer2D _sound;

    private enum BombState
    {
        Idle,
        FuseLit,
        Exploding
    }

    private float _fuseLeft;
    private float _explodeTimeLeft;
    private BombState _state = BombState.Idle;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _explosionAnimation = GetNode<AnimatedSprite2D>("ExplosionAnimation");
        _fire = GetNode<CpuParticles2D>("Fire");
        _smoke = GetNode<CpuParticles2D>("Smoke");
        _sound = GetNode<AudioStreamPlayer2D>("Sound");
        _explosionAnimation.Play("idle");
    }

    public override void _Process(double delta)
    {
        switch (_state)
        {
            case BombState.Idle:
                break;

            case BombState.FuseLit:
                if (_fuseLeft <= 0)
                {
                    Explode();
                }
                else
                {
                    _fuseLeft -= (float)delta;
                }

                break;

            case BombState.Exploding:
                _explodeTimeLeft -= (float)delta;
                if (_explodeTimeLeft <= 0)
                {
                    QueueFree();
                }

                break;
        }
    }

    public void LightFuse()
    {
        _fuseLeft = FuseTime;
        _state = BombState.FuseLit;
        _animatedSprite2D.Play("armed");
    }

    public void Explode()
    {
        _state = BombState.Exploding;
        _animatedSprite2D.Visible = false;
        _fire.Emitting = true;
        _smoke.Emitting = true;
        _explosionAnimation.Play("explode");
        _explosionAnimation.AnimationFinished += () => _explosionAnimation.Visible = false; 
        _explodeTimeLeft = 5.0f;
        _sound.Play();
    }
}