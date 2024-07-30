using Godot;
using System;
using Theadventureofhink.entities.fire;
using Theadventureofhink.utils;

public partial class UndergroundCutscene : Node2D
{
    private HouseFire _houseFire1;
    private HouseFire _houseFire2;
    private HouseFire _houseFire3;
    private HouseFire _oldManHouseFire1;
    private HouseFire _oldManHouseFire2;
    private HouseFire _oldManHouseFire3;

    public override void _Ready()
    {
        _houseFire1 = GetNode<HouseFire>("../House1Fire/HouseFire");
        _houseFire2 = GetNode<HouseFire>("../House1Fire/HouseFire2");
        _houseFire3 = GetNode<HouseFire>("../House1Fire/HouseFire3");
        _oldManHouseFire1 = GetNode<HouseFire>("../House2Fire/HouseFire");
        _oldManHouseFire2 = GetNode<HouseFire>("../House2Fire/HouseFire2");
        _oldManHouseFire3 = GetNode<HouseFire>("../House2Fire/HouseFire3");
    }

    public override void _Process(double delta)
    {
    }

    public void OnBodyEnteredFireTrigger(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _houseFire1.State = FireState.OnFire;
            _houseFire2.State = FireState.OnFire;
            _houseFire3.State = FireState.OnFire;
            _oldManHouseFire1.State = FireState.OnFire;
            _oldManHouseFire2.State = FireState.OnFire;
            _oldManHouseFire3.State = FireState.OnFire;
        }
    }
}