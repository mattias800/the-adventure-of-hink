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

    private bool _isRespawnTeleporting;

    private GameManager _gameManager;
    private GameState _gameState;

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
        _gameState = GetNode<GameState>(Singletons.GameState);
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

        PlatformController.PlayerTurned += OnPlayerTurned;
        PlatformController.PlayerDashStarted += OnPlayerDashStarted;
        PlatformController.PlayerDashStopped += OnPlayerDashStopped;
        PlatformController.PlayerStartedMovingOnGround += OnPlayerStartedMovingOnGround;
        PlatformController.PlayerStoppedMovingOnGround += OnPlayerStoppedMovingOnGround;
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
        _activeController = CharacterControllerType.Platform;
    }

    public void SwitchToOverworld()
    {
        _activeController = CharacterControllerType.Overworld;
    }

    private void OnPlayerDashStarted(Vector2 direction)
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
        CallDeferred(nameof(TurnOffCollisions));

        _animatedSprite.Visible = false;

        _playerDeathTeleportation.PlayTeleporting();

        var duration = GlobalPosition.DistanceTo(spawnWorldPos) / 200;
        duration = Mathf.Clamp(duration, 1.0f, 2.0f);

        var fx = (AudioEffectLowPassFilter)AudioServer.GetBusEffect(AudioServer.GetBusIndex("Music"), 0);

        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetParallel();
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
        CallDeferred(nameof(TurnOnCollisions));
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
                GD.Print("enable platform player");
                PlatformController.Enable();
                break;
            case CharacterControllerType.Overworld:
                GD.Print("enable overworld player");
                OverworldController.Enable();
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