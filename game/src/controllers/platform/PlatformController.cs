using Godot;
using System;
using Godot.NativeInterop;

public partial class PlatformController : Node
{
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

    public float Gravity { get; private set; }

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

    public PlayerDirection CurrentPlayerDirection { get; set; } = PlayerDirection.Right;
    public float TimeSinceNoGround { get; set; } = 0.0f;
    public float TimeUntilWallGrabPossible { get; set; } = 0.0f;
    public float WallGrabTimeLeft { get; set; } = 0.0f;
    public float DashTimeLeft { get; set; } = 0.0f;
    public Vector2 DashDirection { get; set; } = Vector2.Zero;
    public float TimeUntilJumpVelocityResetAllowed { get; set; } = 0.0f;
    public float TimeUntilJumpHorizontalControl { get; set; } = 0.0f;
    public int JumpsLeft { get; set; } = 0;
    public int DashesLeft { get; set; } = 0;
    public int NumDoubleJumps { get; set; } = 1;
    public int NumDashes { get; set; } = 1;
    public bool IsAgainstWall { get; set; } = false;
    public float CoyoteTimeFromGroundLeft { get; set; } = 0.0f;
    public float CoyoteTimeFromWallLeft { get; set; } = 0.0f;
    public Vector2 NormalForLastWallTouched { get; set; } = Vector2.Zero;
    public float VelocityIntoWall { get; set; } = 0.0f;
    public TileMap ActiveTileMap { get; set; } = null;

    public CharacterBody2D Player { get; private set; }
    public AnimatedSprite2D AnimatedSprite { get; private set; }
    public AudioStreamPlayer2D JumpSound { get; private set; }
    public AudioStreamPlayer2D LandSound { get; private set; }
    public AudioStreamPlayer2D DashSound { get; private set; }
    public AudioStreamPlayer2D GrabWallSound { get; private set; }
    public AudioStreamPlayer2D JumpFromWallSound { get; private set; }
    public AudioStreamPlayer2D JumpFromAirSound { get; private set; }
    public RayCast2D WallRayCastLeft { get; private set; }
    public RayCast2D WallRayCastRight { get; private set; }
    public PackedScene? DustBoomScene { get; private set; }

    private PlayerState currentState;

    public PlatformController(
        CharacterBody2D player,
        AnimatedSprite2D animatedSprite,
        AudioStreamPlayer2D jumpSound,
        AudioStreamPlayer2D landSound,
        AudioStreamPlayer2D dashSound,
        AudioStreamPlayer2D grabWallSound,
        AudioStreamPlayer2D jumpFromWallSound,
        AudioStreamPlayer2D jumpFromAirSound,
        PackedScene? dustBoomScene)
    {
        Player = player;
        AnimatedSprite = animatedSprite;
        JumpSound = jumpSound;
        LandSound = landSound;
        DashSound = dashSound;
        GrabWallSound = grabWallSound;
        JumpFromWallSound = jumpFromWallSound;
        JumpFromAirSound = jumpFromAirSound;
        WallRayCastLeft = player.GetNode<RayCast2D>("WallRayCastLeft");
        WallRayCastRight = player.GetNode<RayCast2D>("WallRayCastRight");
        DustBoomScene = dustBoomScene;

        Gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
    }

    public override void _Ready()
    {
        ChangeState(new IdleState(this));
    }

    public override void _PhysicsProcess(double delta)
    {
        currentState.PhysicsProcess(delta);
        UpdatePlayerDirection();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
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
        ChangeState(new IdleState(this));
    }

    public void Disable()
    {
        ChangeState(new DisabledState(this));
    }

    public bool IsNodeIsWallJumpable(Node2D node)
    {
        return WallRayCastRight.GetCollider() is TileMap;
    }

    public Vector2 GetWallJumpDirection(Vector2 wallNormal)
    {
        Vector2 jumpDirection = new Vector2(wallNormal.X, -1);
        return jumpDirection.Normalized();
    }
}