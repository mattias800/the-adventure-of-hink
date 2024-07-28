using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class Camera : Camera2D
{
    public enum CameraState
    {
        Idle,
        Controlled,
        FollowingTarget,
        FollowingPlayer
    }

    public Node2D? Target;
    public CameraState State = CameraState.FollowingPlayer;

    private float _releaseFalloff = 35.0f;
    private float _acceleration = 10.0f;
    private float _maxSpeed = 20.0f;
    public Vector2 Velocity = Vector2.Zero;

    public Vector2 LookAhead = Vector2.Zero;
    public Vector2 LookAheadTarget = Vector2.Zero;

    private Vector2 _viewport = new(320.0f, 180.0f);

    private Vector2 _halfViewport = new(320.0f / 2, 180.0f / 2);

    private CameraManager _cameraManager;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _cameraManager = GetNode<CameraManager>(Singletons.CameraManager);
        SetAnchorMode(AnchorModeEnum.FixedTopLeft);
        State = CameraState.FollowingPlayer;
        _cameraManager.SetCamera(this);
    }

    public override void _Process(double delta)
    {
        if (Target != null)
        {
            switch (State)
            {
                case CameraState.FollowingTarget:
                {
                    var focus = ClampVec2ToLimits(Target.GetGlobalTransform().Origin - _halfViewport);
                    GlobalTransform = new Transform2D(0f, Lerp(GlobalTransform.Origin, focus, 0.1f));
                    break;
                }

                case CameraState.FollowingPlayer:
                {
                    LookAhead = Lerp(LookAhead, LookAheadTarget, 0.005f);
                    var focus = ClampVec2ToLimits(Target.Position - _halfViewport + LookAhead);
                    Position = Lerp(Position, focus, 0.1f);
                    break;
                }

                case CameraState.Controlled:
                {
                    // var input_vector = Input.get_vector("camera_move_left", "camera_move_right", "camera_move_up", "camera_move_down")
                    var targetVector = GetVectorFromCenterToPlayer();
                    CalculateVelocity((float)delta, targetVector);
                    UpdateGlobalPosition((float)delta);
                    break;
                }
            }
        }


        ClampCameraToLimits();
    }

    public override void _PhysicsProcess(double delta)
    {
        ClampCameraToLimits();
    }

    public void SetCameraTarget(Node2D t)
    {
        Target = t;
        LookAheadTarget = new Vector2(0, 0);
    }

    public void JumpToTarget()
    {
        if (Target != null)
        {
            Position = ClampVec2ToLimits(Target.Position - _halfViewport);
        }
    }

    public void OnPlayerTurned(string direction)
    {
        switch (direction)
        {
            case "right":
                LookAheadTarget = new Vector2(50, 0);
                return;
            case "left":
                LookAheadTarget = new Vector2(-50, 0);
                return;
            case "up":
                LookAheadTarget = new Vector2(0, -50);
                return;
            case "down":
                LookAheadTarget = new Vector2(0, 50);
                return;
        }
    }

    private Vector2 GetVectorFromCenterToPlayer()
    {
        if (Target == null)
        {
            throw new Exception("GetVectorFromCenterToPlayer() but Target is null.");
        }

        var x = Target.GetGlobalTransformWithCanvas().Origin - GetViewportRect().GetCenter();
        return x.Normalized();
    }

    private void UpdateGlobalPosition(float delta)
    {
        GlobalPosition += Lerp(
            Velocity,
            Vector2.Zero,
            Mathf.Pow(2, -32 * delta)
        );
    }

    public void ClearCameraLimits()
    {
        var l = 1000000000;
        LimitLeft = -l;
        LimitRight = l;
        LimitTop = -l;
        LimitBottom = l;
    }

    private void ClampCameraToLimits()
    {
        // Prevent camera position to outside of limits.
        // This ensures that background does not move because of limit and position being out of sync.
        // Camera limits must be set first.
        // var zoomed_viewport_size = get_viewport_to_zoom_scale()
        var zoomedViewportSize = new Vector2(320, 180);
        var leftLimit = GetLimit(Side.Left);
        var rightLimit = GetLimit(Side.Right) - zoomedViewportSize.X;
        var topLimit = GetLimit(Side.Top);
        var bottomLimit = GetLimit(Side.Bottom) - zoomedViewportSize.Y;

        Position = new Vector2(Mathf.Clamp(Position.X, leftLimit, rightLimit),
            Mathf.Clamp(Position.Y, topLimit, bottomLimit));
    }

    private Vector2 ClampVec2ToLimits(Vector2 val)
    {
        // Camera limits must be set first.
        var zoomedViewportSize = GetViewportToZoomScale();

        var leftLimit = GetLimit(Side.Left);
        var rightLimit = GetLimit(Side.Right) + zoomedViewportSize.X;
        var topLimit = GetLimit(Side.Top);
        var bottomLimit = GetLimit(Side.Bottom) + zoomedViewportSize.Y;

        return new Vector2(
            Mathf.Clamp(val.X, leftLimit, rightLimit),
            Mathf.Clamp(val.Y, topLimit, bottomLimit)
        );
    }

    private Vector2I GetViewportToZoomScale()
    {
        var zoomVector = GetZoom();
        var zoomedViewportSize = new Vector2I(
            (int)(GetViewport().GetVisibleRect().Size.X / zoomVector.X),
            (int)(GetViewport().GetVisibleRect().Size.Y / zoomVector.Y)
        );

        return zoomedViewportSize;
    }

    private void CalculateVelocity(float delta, Vector2 direction)
    {
        Velocity += direction * _acceleration * delta;

        if (direction.X == 0)
        {
            Velocity.X = (float)Mathf.Lerp(0.0, Velocity.X, Mathf.Pow(2, -_releaseFalloff * delta));
        }

        if (direction.Y == 0)
        {
            Velocity.Y = (float)Mathf.Lerp(0.0, Velocity.Y, Mathf.Pow(2, -_releaseFalloff * delta));
        }

        Velocity.X = Mathf.Clamp(
            Velocity.X,
            -_maxSpeed,
            _maxSpeed
        );

        Velocity.Y = Mathf.Clamp(
            Velocity.Y,
            -_maxSpeed,
            _maxSpeed
        );
    }

    private Vector2I GetTilemapSize(TileMapLayer sourceTilemapLayer)
    {
        var tilemapRect = sourceTilemapLayer.GetUsedRect();
        return new Vector2I(
            tilemapRect.End.X - tilemapRect.Position.X,
            tilemapRect.End.Y - tilemapRect.Position.Y
        );
    }

    private Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
    }

    private float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}