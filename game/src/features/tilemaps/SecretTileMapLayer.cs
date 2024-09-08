using Godot;
using System;
using Theadventureofhink.autoloads;

public partial class SecretTileMapLayer : TileMapLayer
{
    private GameManager _gameManager;

    private State _state = State.Solid;
    private float _currentOpacity = 1.0f;

    enum State
    {
        Solid,
        Opaque
    }

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
    }

    public override void _Process(double delta)
    {
        UpdateState();

        switch (_state)
        {
            case State.Opaque:
                _currentOpacity = Mathf.Lerp(_currentOpacity, 0.0f, (float)delta * 10f);
                break;
            case State.Solid:
                _currentOpacity = Mathf.Lerp(_currentOpacity, 1.0f, (float)delta * 10f);
                break;
        }

        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, _currentOpacity);
    }

    private void UpdateState()
    {
        var p = _gameManager.Player.GlobalPosition;
        var c = LocalToMap(p - GlobalPosition);
        var playerIsInside = GetCellSourceId(c) >= 0;
        if (playerIsInside)
        {
            _state = State.Opaque;
        }
        else
        {
            _state = State.Solid;
        }
    }
}