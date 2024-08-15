using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

public partial class Bird : Node2D
{
    public enum BirdState
    {
        Idle,
        FlyingAway
    }

    [Export] public BirdState State = BirdState.Idle;
    [Export] public Vector2 FlyingDirection = new(1, -1);

    [Export] public Area2D PlayerFlyArea;
    [Export] public float FlyAwayStartSpeed = 40.0f;
    [Export] public float FlyMaxSpeed = 100.0f;

    private float _currentSpeed = 40.0f;
    private AnimatedSprite2D _animatedSprite2D;

    private double _distanceFlown;
    private float _animationSpeed = RandomHelper.RandfRange(0.7f, 1.3f);

    private CameraManager _cameraManager;
    
    public override void _Ready()
    {
        _cameraManager = GetNode<CameraManager>(Singletons.CameraManager);   
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        _currentSpeed = FlyAwayStartSpeed;
        if (PlayerFlyArea != null)
        {
            PlayerFlyArea.BodyEntered += FlyAwayIfPlayer; 
        }

        _animatedSprite2D.Play("idle", _animationSpeed);

    }
    
    public override void _Process(double delta)
    {
        switch (State)
        {
            case BirdState.Idle:
            _animatedSprite2D.Play("idle", _animationSpeed);
            break;
            case BirdState.FlyingAway:
            _animatedSprite2D.Play("fly", _animationSpeed);
            break;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_cameraManager.Camera == null)
        {
            return;
        }
        
        switch (State)
        {
            case BirdState.Idle:
                break;
            case BirdState.FlyingAway:
                _currentSpeed ++;
                if (_currentSpeed > FlyMaxSpeed)
                {
                    _currentSpeed = FlyMaxSpeed;
                }

                var d = FlyingDirection * _currentSpeed * (float)delta;
                GlobalPosition += d;
                _distanceFlown += d.Length();

                var distanceToCamera = (_cameraManager.Camera.GlobalPosition - GlobalPosition).Abs();
                if (distanceToCamera.X > 400 || distanceToCamera.Y > 200)
                {
                    QueueFree();
                }

                break;
        }
    }

    private void FlyAwayIfPlayer(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            GD.Print("Player approached bird");
            FlyAway();
        }
        
    }

    public void FlyAway()
    {
        if (FlyingDirection.X < 0)
        {
            _animatedSprite2D.FlipH = true;
        }

        State = BirdState.FlyingAway;
    }


}