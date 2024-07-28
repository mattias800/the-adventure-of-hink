using Godot;
using System.Linq;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

public partial class Boomerang : Node2D
{
    [Signal]
    public delegate void CollidedWithPlayerEventHandler();

    [Signal]
    public delegate void HitBodyEventHandler(Node2D body);

    public BoomerangState State = BoomerangState.GoingOut;

    // Enum for State
    public enum BoomerangState
    {
        GoingOut,
        GoingBack,
        Stuck
    }

    // Nodes
    private Area2D _area2d;
    private Sprite2D _sprite2d;
    private AudioStreamPlayer2D _startSound;
    private AudioStreamPlayer2D _stuckSound;
    private AudioStreamPlayer2D _flyingSound;

    // Movement
    private Vector2 _currentVelocity = new Vector2(1, 0);
    private float _maxSpeed = 200.0f;

    private float _outgoingProgress = 0.0f;
    private float _outgoingThreshold = 0.2f;
    private float _angularVelocityMultiplier = 0.07f;

    // Visual
    private float _rotationSpeed = 4.0f;
    private bool _rotating = true;

    // Singletons
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);

        // Initialize onready variables
        _area2d = GetNode<Area2D>("Area2D");
        _sprite2d = GetNode<Sprite2D>("Sprite2D");
        _startSound = GetNode<AudioStreamPlayer2D>("StartSound");
        _stuckSound = GetNode<AudioStreamPlayer2D>("StuckSound");
        _flyingSound = GetNode<AudioStreamPlayer2D>("FlyingSound");
    }

    public void Throw(Vector2 direction)
    {
        _currentVelocity = direction;
        _outgoingProgress = 0.0f;
        EnterState(BoomerangState.GoingOut);
    }

    public override void _Process(double delta)
    {
        switch (State)
        {
            case BoomerangState.GoingOut:
                _sprite2d.Rotate(Mathf.Pi * _rotationSpeed * (float)delta);
                break;

            case BoomerangState.GoingBack:
                _sprite2d.Rotate(Mathf.Pi * _rotationSpeed * (float)delta);
                GlobalRotation = 0.0f;
                break;

            case BoomerangState.Stuck:
                // Do nothing
                break;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (State)
        {
            case BoomerangState.GoingOut:
                var bodiesOut = CollisionUtil.BodiesExceptPlayer(_area2d.GetOverlappingBodies()).ToList();

                if (bodiesOut.Count > 0)
                {
                    EmitSignal(SignalName.HitBody, bodiesOut[0]);
                    GD.Print("Got stuck when going back");
                    EnterState(BoomerangState.Stuck);
                    return;
                }

                _outgoingProgress += (float)delta;
                GlobalPosition += _currentVelocity * _maxSpeed * (float)delta;

                if (_outgoingProgress >= _outgoingThreshold)
                {
                    EnterState(BoomerangState.GoingBack);
                }

                break;

            case BoomerangState.GoingBack:
                var bodiesBack = _area2d.GetOverlappingBodies();

                if (CollisionUtil.BodiesContainPlayer(bodiesBack))
                {
                    EmitSignal(SignalName.CollidedWithPlayer);
                    QueueFree();
                    return;
                }

                if (bodiesBack.Count > 0)
                {
                    EmitSignal(SignalName.HitBody, bodiesBack[0]);
                    GD.Print("Got stuck when going back");
                    EnterState(BoomerangState.Stuck);
                    return;
                }

                var directionToPlayer = _gameManager.Player.GlobalPosition - GlobalPosition;
                _currentVelocity += directionToPlayer.Normalized() * _angularVelocityMultiplier;

                if (_currentVelocity.Length() > 1.0f)
                {
                    _currentVelocity = _currentVelocity.Normalized();
                }

                GlobalPosition += _currentVelocity * _maxSpeed * (float)delta;
                break;

            case BoomerangState.Stuck:
                // Do nothing
                break;
        }
    }

    private void EnterState(BoomerangState next)
    {
        State = next;
        GD.Print("Boomerang EnterState: " + State);
        switch (State)
        {
            case BoomerangState.GoingOut:
                _startSound.Play(0.05f);
                _flyingSound.Play();
                break;

            case BoomerangState.GoingBack:
                // Do nothing
                break;

            case BoomerangState.Stuck:
                _stuckSound.Play();
                _flyingSound.Stop();
                _sprite2d.Rotation = 0.0f;
                break;
        }
    }
}