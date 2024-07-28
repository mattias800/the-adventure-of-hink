using Godot;
using System;

public partial class BoomerangController : Node2D
{
    [Export] public PackedScene BoomerangScene;

    private int _spawnPositionOffset = 8;
    private Vector2 _positionOffsetFromPlayerFeet = new(0, -4);
    private Vector2 _directionWhenPlayerIdle = new(1, 0);

    private Node2D? _boomerangInstance;

    public override void _Ready()
    {
        // Initialization if needed
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("throw"))
        {
            var heldDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            SpawnBoomerang(heldDirection);
        }
    }

    private void SpawnBoomerang(Vector2 heldDirection)
    {
        var direction = heldDirection.Length() > 0.1 ? heldDirection : _directionWhenPlayerIdle;

        if (_boomerangInstance != null && IsInstanceValid(_boomerangInstance))
        {
            if (_boomerangInstance is Boomerang { State: Boomerang.BoomerangState.Stuck })
            {
                _boomerangInstance.QueueFree();
            }
        }

        _boomerangInstance = (Boomerang)BoomerangScene.Instantiate();
        _boomerangInstance.GlobalPosition = GetSpawnGlobalPosition(direction);
        GetTree().Root.AddChild(_boomerangInstance);

        if (_boomerangInstance is Boomerang boomerangInstance)
        {
            boomerangInstance.Throw(direction);
        }
    }

    private void OnPlayerPlayerTurned(string playerDirection)
    {
        if (playerDirection == "left")
        {
            _directionWhenPlayerIdle = new Vector2(-1, 0);
        }
        else
        {
            _directionWhenPlayerIdle = new Vector2(1, 0);
        }
    }

    private Vector2 GetSpawnGlobalPosition(Vector2 direction)
    {
        Vector2 startPosition = GlobalPosition + direction * _spawnPositionOffset;
        return startPosition + _positionOffsetFromPlayerFeet;
    }
}