using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.fire;
using Theadventureofhink.utils;

namespace Theadventureofhink.world.world_01.hometown.fireplace;

public partial class Fireplace : Sprite2D
{
    private GameManager _gameManager;
    private HouseFire _houseFire;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>(Singletons.GameManager);
        _houseFire = GetNode<HouseFire>("HouseFire");
        _houseFire.State = FireState.Off;
    }

    public void StartFire()
    {
        _houseFire.State = FireState.OnFire;
    }

    public void OnEnterFire(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body) && _houseFire.State == FireState.OnFire)
        {
            _gameManager.RespawnPlayer();
        }
    }
}