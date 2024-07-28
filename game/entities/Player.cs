using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class Player : CharacterBody2D
{
    [Export]
    public PackedScene? DustBoomScene;

    // Signals
    [Signal]
    public delegate void PlayerDisabledEventHandler();
    [Signal]
    public delegate void PlayerEnabledEventHandler();
    [Signal]
    public delegate void PlayerTurnedEventHandler(string direction);
    [Signal]
    public delegate void PlayerDashStartedEventHandler(Vector2 direction);
    [Signal]
    public delegate void PlayerDashStoppedEventHandler();
    [Signal]
    public delegate void PlayerStartedMovingOnGroundEventHandler();
    [Signal]
    public delegate void PlayerStoppedMovingOnGroundEventHandler();

    // Nodes
    private PlayerDeathTeleportation _playerDeathTeleportation;
    private AnimatedSprite2D _animatedSprite;
    private AudioStreamPlayer2D _playerJumpSound;
    private AudioStreamPlayer2D _playerLandSound;
    private AudioStreamPlayer2D _playerGrabWallSound;
    private AudioStreamPlayer2D _playerJumpFromWallSound;
    private AudioStreamPlayer2D _playerDashSound;
    private AudioStreamPlayer2D _deathBoomSound;
    private AudioStreamPlayer2D _deathAppearSound;
    private CollisionShape2D _collisionShape;
    private CollisionShape2D _bounceShape;
    private RayCast2D _squishCastRight;
    private RayCast2D _squishCastLeft;
    private RayCast2D _squishCastUp;
    private RayCast2D _squishCastDown;
    private RayCast2D _wallRayCastLeft;
    private RayCast2D _wallRayCastRight;
    private Node2D _roomDetection;

    private PlatformController _platformController;
    private OverworldController _overworldController;

    private bool _enabled = false;
    private bool _isRespawnTeleporting = false;

    private GameManager _gameManager;

    public enum CharacterControllerType
    {
        Platform,
        Overworld
    }

    public enum PlayerState
    {
        Active,
        DeathTeleportation
    }

    private CharacterControllerType _activeController = CharacterControllerType.Platform;
    private PlayerState _state = PlayerState.Active;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _playerDeathTeleportation = GetNode<PlayerDeathTeleportation>("PlayerDeathTeleportation");
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _playerJumpSound = GetNode<AudioStreamPlayer2D>("PlayerJumpSound");
        _playerLandSound = GetNode<AudioStreamPlayer2D>("PlayerLandSound");
        _playerGrabWallSound = GetNode<AudioStreamPlayer2D>("PlayerGrabWallSound");
        _playerJumpFromWallSound = GetNode<AudioStreamPlayer2D>("PlayerJumpFromWallSound");
        _playerDashSound = GetNode<AudioStreamPlayer2D>("PlayerDashSound");
        _deathBoomSound = GetNode<AudioStreamPlayer2D>("DeathBoomSound");
        _deathAppearSound = GetNode<AudioStreamPlayer2D>("DeathAppearSound");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _bounceShape = GetNode<CollisionShape2D>("BounceShape/CollisionShape2D");
        _squishCastRight = GetNode<RayCast2D>("SquishCastRight");
        _squishCastLeft = GetNode<RayCast2D>("SquishCastLeft");
        _squishCastUp = GetNode<RayCast2D>("SquishCastUp");
        _squishCastDown = GetNode<RayCast2D>("SquishCastDown");
        _wallRayCastLeft = GetNode<RayCast2D>("WallRayCastLeft");
        _wallRayCastRight = GetNode<RayCast2D>("WallRayCastRight");
        _roomDetection = GetNode<Node2D>("RoomDetection");
        
        _platformController = new PlatformController(
            this,
            _animatedSprite,
            _playerJumpSound,
            _playerLandSound,
            _playerDashSound,
            _playerGrabWallSound,
            _playerJumpFromWallSound,
            GetNode<AudioStreamPlayer2D>("PlayerJumpFromAirSound"),
            DustBoomScene
        );

        _platformController.PlayerTurned += OnPlayerTurned;
        _platformController.PlayerDashStarted += OnPlayerDashStarted;
        _platformController.PlayerDashStopped += OnPlayerDashStopped;
        _platformController.PlayerStartedMovingOnGround += OnPlayerStartedMovingOnGround;
        _platformController.PlayerStoppedMovingOnGround += OnPlayerStoppedMovingOnGround;
        _platformController._Ready();
        _overworldController = new OverworldController(this, _animatedSprite);
        _overworldController._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        GD.Print("Player _PhysicsProcess enabled=" + _enabled);
        if (!_enabled)
            return;

        switch (_state)
        {
            case PlayerState.Active:
                CheckSquish();
                switch (_activeController)
                {
                    case CharacterControllerType.Platform:
                        GD.Print("physics platform");
                        _platformController._PhysicsProcess(delta);
                        break;
                    case CharacterControllerType.Overworld:
                        GD.Print("physics overworld");
                        _overworldController._PhysicsProcess(delta);
                        break;
                }
                break;
            case PlayerState.DeathTeleportation:
                break;
        }
    }

    public void SwitchToPlatform()
    {
        GD.Print("SwitchToPlatform");
        _activeController = CharacterControllerType.Platform;
    }

    public void SwitchToOverworld()
    {
        GD.Print("SwitchToOverworld");
        _activeController = CharacterControllerType.Overworld;
    }

    private void OnPlayerDashStarted(Vector2 direction)
    {
        GetNode<AnimationPlayer>("DashAnimation").Play();
        EmitSignal(SignalName.PlayerDashStarted, direction);
    }

    public void DeathTeleport(Vector2 spawnWorldPos)
    {
        if (_isRespawnTeleporting)
            return;

        GD.Print("Player died, teleporting!");
        _isRespawnTeleporting = true;
        _deathBoomSound.Play();
        Disable();
        TurnOffCollisions();

        _animatedSprite.Visible = false;
        
        _playerDeathTeleportation.PlayTeleporting();

        var duration = GlobalPosition.DistanceTo(spawnWorldPos) / 200;
        duration = Mathf.Clamp(duration, 1.0f, 2.0f);

        var fx = (AudioEffectLowPassFilter)AudioServer.GetBusEffect(AudioServer.GetBusIndex("Music"), 0);

        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetParallel(true);
        tween.TweenProperty(this, "global_position", spawnWorldPos, duration);
        tween.TweenMethod(new Callable(fx, "SetCutoff"), 0, 500, duration).SetTrans(Tween.TransitionType.Circ);
        tween.TweenMethod(new Callable(fx, "SetResonance"), 2.0, 0.5, duration).SetTrans(Tween.TransitionType.Circ);
        tween.Finished += OnDeathTeleportationDone;
    }

    private void OnDeathTeleportationDone()
    {
        GD.Print("death tele done");
        _isRespawnTeleporting = false;
        var fx = (AudioEffectLowPassFilter)AudioServer.GetBusEffect(AudioServer.GetBusIndex("Music"), 0);
        fx.SetCutoff(20500);
        fx.SetResonance(0.5f);
        _deathAppearSound.Play();
        
        _playerDeathTeleportation.PlayPlayerAppearing();
        TurnOnCollisions();
        _animatedSprite.Visible = true;
        Velocity = Vector2.Zero;
        Enable();
    }

    public void Enable()
    {
        _enabled = true;
        switch (_activeController)
        {
            case CharacterControllerType.Platform:
                GD.Print("enable platform player");
                _platformController.Enable();
                break;
            case CharacterControllerType.Overworld:
                GD.Print("enable overworld player");
                _overworldController.Enable();
                break;
        }
        SetPhysicsProcess(true);
        EmitSignal(SignalName.PlayerEnabled);
    }

    private void TurnOffCollisions()
    {
        _collisionShape.Disabled = true;
        _bounceShape.Disabled = true;
    }

    private void TurnOnCollisions()
    {
        _collisionShape.Disabled = false;
        _bounceShape.Disabled = false;
    }

    public void Disable()
    {
        GD.Print("disable player");
        _enabled = false;
        _platformController.Disable();
        _overworldController.Disable();
        EmitSignal(SignalName.PlayerDisabled);
    }

    public void TriggerForce(Vector2 force)
    {
        switch (_activeController)
        {
            case CharacterControllerType.Platform:
                _platformController.TriggerForce(force);
                break;
        }
    }

    public void OnHitJumpSource()
    {
        switch (_activeController)
        {
            case CharacterControllerType.Platform:
                _platformController.OnHitJumpSource();
                break;
        }
    }

    private async void CheckSquish()
    {
        bool squishSides = _squishCastLeft.IsColliding() && _squishCastRight.IsColliding();
        bool squishUpdown = _squishCastUp.IsColliding() && _squishCastDown.IsColliding();
        if (squishSides || squishUpdown)
        {
            await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
            GD.Print("Player squished!");
            _gameManager.RespawnPlayer();
        }
    }

    private void OnPlayerTurned(string direction)
    {
        EmitSignal(SignalName.PlayerTurned, direction);
    }

    private void OnPlayerDashStopped()
    {
        EmitSignal(SignalName.PlayerDashStopped);
    }

    private void OnPlayerStartedMovingOnGround()
    {
        EmitSignal(SignalName.PlayerStartedMovingOnGround);
    }

    private void OnPlayerStoppedMovingOnGround()
    {
        EmitSignal(SignalName.PlayerStoppedMovingOnGround);
    }
}