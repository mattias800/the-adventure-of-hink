using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.portals;
using Theadventureofhink.world;

public partial class BlockingPortal : StaticBody2D, IPortal
{
    [Export] public bool Enabled = true;

    [Export] public Stage NextStage;

    [Export] public string TargetPortalName;

    private GameManager _gameManager;
    private ContextBubble _contextBubble;

    private bool _playerIsTouching;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _contextBubble = GetNode<ContextBubble>("ContextBubble");
        _contextBubble.Scale = Vector2.Zero;
        _contextBubble.SetLabel(Stages.GetStateInfo(NextStage).MapLabel);
    }

    public override void _Process(double delta)
    {
        var targetSize = _playerIsTouching ? Vector2.One : Vector2.Zero;

        _contextBubble.Scale = _contextBubble.Scale.Lerp(targetSize, (float)delta * 20);

        if (_playerIsTouching)
        {
            if (Input.IsActionPressed("interact"))
            {
                _gameManager.OnPlayerEnteredPortal(this);
            }
        }
    }

    public Stage GetNextStage()
    {
        return NextStage;
    }

    public string GetTargetPortalName()
    {
        return TargetPortalName;
    }

    public new string GetName()
    {
        return Name;
    }

    public void OnPlayerStartTouching()
    {
        _playerIsTouching = true;
    }

    public void OnPlayerStopTouching()
    {
        _playerIsTouching = false;
    }

    public Vector2 GetSpawnPosition()
    {
        return GlobalPosition;
    }
}