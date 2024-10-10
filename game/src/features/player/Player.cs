using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.features.player.platform;
using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player;

public partial class Player : CharacterBody2D
{
    [Export] public PlatformController PlatformController;

    [Export] public OverworldController OverworldController;


    [Export] public PackedScene DustBoomScene;

    public bool Enabled;

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

    private AudioStreamPlayer2D _deathBoomSound;
    private AudioStreamPlayer2D _deathAppearSound;
    private CollisionShape2D _collisionShape;
    private CollisionShape2D _bounceShape;
    private RayCast2D _squishCastRight;
    private RayCast2D _squishCastLeft;
    private RayCast2D _squishCastUp;
    private RayCast2D _squishCastDown;

    private float _timeUntilDelayedRespawn;
    private bool _isRespawnTeleporting;

    private AudioEffectLowPassFilter _lowPassFilter;
    private GameManager _gameManager;
    private GameStateManager _gameStateManager;

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
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
        _playerDeathTeleportation = GetNode<PlayerDeathTeleportation>("PlayerDeathTeleportation");
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        _deathBoomSound = GetNode<AudioStreamPlayer2D>("DeathBoomSound");
        _deathAppearSound = GetNode<AudioStreamPlayer2D>("DeathAppearSound");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _bounceShape = GetNode<CollisionShape2D>("BounceShape/CollisionShape2D");
        _squishCastRight = GetNode<RayCast2D>("SquishCastRight");
        _squishCastLeft = GetNode<RayCast2D>("SquishCastLeft");
        _squishCastUp = GetNode<RayCast2D>("SquishCastUp");
        _squishCastDown = GetNode<RayCast2D>("SquishCastDown");

        // _lowPassFilter = (AudioEffectLowPassFilter)AudioServer.GetBusEffect(AudioServer.GetBusIndex("Music"), 0);

        PlatformController.PlayerTurned += OnPlayerTurned2;
        PlatformController.PlayerDashStarted += OnPlayerDashStarted2;
        PlatformController.PlayerDashStopped += OnPlayerDashStopped2;
        PlatformController.PlayerStartedMovingOnGround += OnPlayerStartedMovingOnGround2;
        PlatformController.PlayerStoppedMovingOnGround += OnPlayerStoppedMovingOnGround2;
    }

    public override void _Process(double delta)
    {
        if (_timeUntilDelayedRespawn > 0.0f)
        {
            _timeUntilDelayedRespawn -= (float)delta;
            if (_timeUntilDelayedRespawn <= 0.0f)
            {
                Callable.From(() => _gameManager.RespawnPlayer()).CallDeferred();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Enabled)
            return;

        switch (_state)
        {
            case PlayerState.Active:
                CheckSquish();
                break;
            case PlayerState.DeathTeleportation:
                break;
        }
    }

    public void SwitchToPlatform()
    {
        PlatformController.Enable();
        OverworldController.Disable();
        _activeController = CharacterControllerType.Platform;
    }

    public void SwitchToOverworld()
    {
        PlatformController.Disable();
        OverworldController.Enable();
        _activeController = CharacterControllerType.Overworld;
    }

    private void OnPlayerDashStarted2(Vector2 direction)
    {
        GetNode<AnimatedSprite2D>("DashAnimation").Play();
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
        Callable.From(TurnOffCollisions).CallDeferred();

        _animatedSprite.Visible = false;

        _playerDeathTeleportation.PlayTeleporting();

        var duration = GlobalPosition.DistanceTo(spawnWorldPos) / 200;
        duration = Mathf.Clamp(duration, 1.0f, 2.0f);
        
        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetParallel();
        tween.TweenProperty(this, "global_position", spawnWorldPos, duration);
        if (_lowPassFilter != null)
        {
            tween.TweenMethod(new Callable(_lowPassFilter, "SetCutoff"), 0, 500, duration)
                .SetTrans(Tween.TransitionType.Circ);
            tween.TweenMethod(new Callable(_lowPassFilter, "SetResonance"), 2.0, 0.5, duration)
                .SetTrans(Tween.TransitionType.Circ);
        }

        tween.Finished += OnDeathTeleportationDone;
    }

    private void OnDeathTeleportationDone()
    {
        GD.Print("death tele done");
        _isRespawnTeleporting = false;

        if (_lowPassFilter != null)
        {
            _lowPassFilter.SetCutoff(20500);
            _lowPassFilter.SetResonance(0.5f);
        }

        _deathAppearSound.Play();

        _playerDeathTeleportation.PlayPlayerAppearing();
        Callable.From(TurnOnCollisions).CallDeferred();

        _animatedSprite.Visible = true;
        Velocity = Vector2.Zero;
        Enable();
    }

    public void Enable()
    {
        Enabled = true;
        switch (_activeController)
        {
            case CharacterControllerType.Platform:
                GD.Print("Enable PlatformController.");
                PlatformController.Enable();
                OverworldController.Disable();
                break;
            case CharacterControllerType.Overworld:
                GD.Print("Enable OverworldController.");
                PlatformController.Disable();
                OverworldController.Enable();
                break;
        }

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
        GD.Print("Disable player.");
        Enabled = false;
        PlatformController.Disable();
        OverworldController.Disable();
        EmitSignal(SignalName.PlayerDisabled);
    }

    public void TriggerForce(Vector2 force)
    {
        switch (_activeController)
        {
            case CharacterControllerType.Platform:
                PlatformController.TriggerForce(force);
                break;
        }
    }

    public void OnHitJumpSource()
    {
        switch (_activeController)
        {
            case CharacterControllerType.Platform:
                PlatformController.OnHitJumpSource();
                break;
        }
    }

    private void CheckSquish()
    {
        var squishSides = _squishCastLeft.IsColliding() && _squishCastRight.IsColliding();
        var squishUpDown = _squishCastUp.IsColliding() && _squishCastDown.IsColliding();

        if (squishSides || squishUpDown)
        {
            _timeUntilDelayedRespawn = 0.05f;

            if (squishSides)
            {
                GD.Print("Player squished between " + (_squishCastLeft.GetCollider() as Node)?.Name + " and " +
                         (_squishCastRight.GetCollider() as Node)?.Name);
            }

            if (squishUpDown)
            {
                GD.Print("Player squished between " + (_squishCastUp.GetCollider() as Node)?.Name + " and " +
                         (_squishCastDown.GetCollider() as Node)?.Name);
            }
        }
    }

    private void OnPlayerTurned2(string direction)
    {
        EmitSignal(SignalName.PlayerTurned, direction);
    }

    private void OnPlayerDashStopped2()
    {
        EmitSignal(SignalName.PlayerDashStopped);
    }

    private void OnPlayerStartedMovingOnGround2()
    {
        EmitSignal(SignalName.PlayerStartedMovingOnGround);
    }

    private void OnPlayerStoppedMovingOnGround2()
    {
        EmitSignal(SignalName.PlayerStoppedMovingOnGround);
    }
}