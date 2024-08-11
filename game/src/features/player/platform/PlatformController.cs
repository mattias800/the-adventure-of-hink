using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.features.player.platform.states;
using Theadventureofhink.game_state;

namespace Theadventureofhink.features.player.platform;

public partial class PlatformController : Node2D
{
    [Export] public CharacterBody2D Player;

    [Export] public WallClimbableProvider WallClimbableProvider;

    [Export] public AnimatedSprite2D AnimatedSprite;

    [Export] public AudioStreamPlayer2D JumpSound;

    [Export] public AudioStreamPlayer2D LandSound;

    [Export] public AudioStreamPlayer2D DashSound;

    [Export] public AudioStreamPlayer2D GrabWallSound;

    [Export] public AudioStreamPlayer2D JumpFromWallSound;

    [Export] public AudioStreamPlayer2D JumpFromAirSound;

    [Export] public RayCast2D WallRayCastLeft;

    [Export] public RayCast2D WallRayCastRight;

    [Export] public PackedScene DustBoomScene;

    public bool Enabled;

    [Signal]
    public delegate void PlayerTurnedEventHandler(string direction);

    [Signal]
    public delegate void PlayerStartedMovingOnGroundEventHandler();

    [Signal]
    public delegate void PlayerStoppedMovingOnGroundEventHandler();

    [Signal]
    public delegate void PlayerDashStartedEventHandler(Vector2 direction);

    [Signal]
    public delegate void PlayerDashStoppedEventHandler();

    public float Speed = 80.0f;
    public float DashSpeed = 400.0f;
    public float DashTime = 0.1f;
    public float MaxFallSpeed = 250.0f;
    public float JumpHorizontalSpeed = 7.0f;
    public float MaxHorizontalSpeed = 80.0f;
    public float JumpVelocity = 150.0f;
    public float JumpReleaseVelocity = 100.0f;
    public float WallGrabTimeLimit = 5.0f;
    public float WallClimbSpeed = 40.0f;
    public float CoyoteTimeLimit = 0.15f;

    public float Gravity;

    public enum JumpSource
    {
        Ground,
        Wall,
        Air
    }

    public enum PlayerDirection
    {
        Left,
        Right
    }

    public PlayerDirection CurrentPlayerDirection = PlayerDirection.Right;
    public float TimeSinceNoGround;
    public float TimeUntilWallGrabPossible;
    public float WallGrabTimeLeft;
    public float DashTimeLeft;
    public Vector2 DashDirection = Vector2.Zero;
    public float TimeUntilJumpVelocityResetAllowed;
    public float TimeUntilJumpHorizontalControl;
    public int JumpsLeft;
    public int DashesLeft;
    public int NumDoubleJumps = 1;
    public int NumDashes = 1;
    public bool IsAgainstWall = false;
    public float CoyoteTimeFromGroundLeft;
    public float CoyoteTimeFromWallLeft;
    public Vector2 NormalForLastWallTouched = Vector2.Zero;
    public float VelocityIntoWall;

    private PlayerState _currentState;
    private PlayerSkillsState _playerSkillsState;

    public override void _Ready()
    {
        Gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        _playerSkillsState = GetNode<GameState>(Singletons.GameState).PlayerState.PlayerSkillsState;
        ChangeState(new IdleState(this));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_playerSkillsState == null)
        {
            GD.PrintErr("PlatformController got _playerSkillsState that is null.");
            return;
        }

        _currentState?.PhysicsProcess(delta, _playerSkillsState);
        UpdatePlayerDirection();
    }

    public void ChangeState(PlayerState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void UpdatePlayerDirection()
    {
        if (Player.Velocity.X > 0)
        {
            AnimatedSprite.FlipH = false;
            if (CurrentPlayerDirection == PlayerDirection.Left)
            {
                EmitSignal(SignalName.PlayerTurned, "right");
            }

            CurrentPlayerDirection = PlayerDirection.Right;
        }
        else if (Player.Velocity.X < 0)
        {
            AnimatedSprite.FlipH = true;
            if (CurrentPlayerDirection == PlayerDirection.Right)
            {
                EmitSignal(SignalName.PlayerTurned, "left");
            }

            CurrentPlayerDirection = PlayerDirection.Left;
        }
    }

    public void AddVelocityX(float val)
    {
        Player.Velocity = new Vector2(
            Mathf.Clamp(Player.Velocity.X + val, -MaxHorizontalSpeed, MaxHorizontalSpeed),
            Player.Velocity.Y
        );
    }

    public void TriggerJump(JumpSource jumpSource)
    {
        switch (jumpSource)
        {
            case JumpSource.Ground:
                JumpSound.Play();
                TimeSinceNoGround = 0.0f;
                TimeUntilWallGrabPossible = 0.1f;
                DashesLeft = NumDashes;
                JumpsLeft = NumDoubleJumps;
                TimeUntilJumpVelocityResetAllowed = 0.0f;
                TimeUntilJumpHorizontalControl = 0.0f;
                break;
            case JumpSource.Air:
                JumpFromAirSound.Play();
                JumpsLeft--;
                TimeUntilJumpVelocityResetAllowed = 0.5f;
                TimeUntilJumpHorizontalControl = 0.1f;
                break;
            case JumpSource.Wall:
                JumpFromWallSound.Play();
                TimeUntilWallGrabPossible = 0.1f;
                DashesLeft = NumDashes;
                JumpsLeft = NumDoubleJumps;
                TimeUntilJumpVelocityResetAllowed = 1.0f;
                TimeUntilJumpHorizontalControl = 0.15f;
                break;
        }

        SpawnDustBoom();
        ChangeState(new JumpingState(this));
        JumpSound.Play();
    }

    public void TriggerDash()
    {
        DashSound.Play();
        DashDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        if (DashDirection == Vector2.Zero)
        {
            DashDirection = new Vector2(CurrentPlayerDirection == PlayerDirection.Right ? 1 : -1, 0);
        }

        DashTimeLeft = DashTime;
        DashesLeft--;
        EmitSignal(SignalName.PlayerDashStarted, DashDirection);
        ChangeState(new DashingState(this));
    }

    public void TriggerForce(Vector2 force)
    {
        GD.Print("trigger_force");
        GD.Print(force);
        Player.Velocity = force;
        DashesLeft = NumDashes;
        JumpsLeft = NumDoubleJumps;
        TimeSinceNoGround = 0.0f;
        WallGrabTimeLeft = WallGrabTimeLimit;
        TimeUntilJumpVelocityResetAllowed = 5;
        ChangeState(new JumpingState(this));
    }

    public void OnHitJumpSource()
    {
        Player.Velocity = new Vector2(Player.Velocity.X, -JumpVelocity);
        TriggerJump(JumpSource.Ground);
        TimeUntilJumpVelocityResetAllowed = 0.2f;
    }

    public void SpawnDustBoom()
    {
        if (DustBoomScene == null)
        {
            GD.Print("DustBoomScene is null");
            return;
        }

        var dustInstance = DustBoomScene.Instantiate<Node2D>();
        dustInstance.GlobalPosition = Player.GlobalPosition + new Vector2(0, 2);
        Player.GetTree().Root.AddChild(dustInstance);
    }

    public void Enable()
    {
        Enabled = true;
        ChangeState(new IdleState(this));
    }

    public void Disable()
    {
        Enabled = false;
        ChangeState(new DisabledState(this));
    }

    public bool IsNodeIsWallJumpable(Node2D node)
    {
        return WallRayCastRight.GetCollider() is TileMapLayer;
    }

    public Vector2 GetWallJumpDirection(Vector2 wallNormal)
    {
        Vector2 jumpDirection = new Vector2(wallNormal.X, -1);
        return jumpDirection.Normalized();
    }
}