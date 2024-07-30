using Godot;
using System;
using System.Collections.Specialized;
using Theadventureofhink.autoloads;
using Theadventureofhink.entities.fire;
using Theadventureofhink.game_state;
using Theadventureofhink.utils;

public partial class Hometown : Node2D
{
    private Room _room;
    private Room _room2;
    private Room _room1Underground;
    private Room _room2Underground;
    private Node2D _attackingState;

    private HouseFire _houseFire1;
    private HouseFire _houseFire2;
    private HouseFire _houseFire3;
    private HouseFire _oldManHouseFire1;
    private HouseFire _oldManHouseFire2;
    private HouseFire _oldManHouseFire3;

    private MusicManager _musicManager;
    private GameState _gameState;

    public override void _Ready()
    {
        _musicManager = GetNode<MusicManager>(Singletons.MusicManager);
        _gameState = GetNode<GameState>(Singletons.GameState);

        _room = GetNode<Room>("Rooms/Room");
        _room2 = GetNode<Room>("Rooms/Room2");
        _room1Underground = GetNode<Room>("Rooms/Room1Underground");
        _room2Underground = GetNode<Room>("Rooms/Room2Underground");
        _attackingState = GetNode<Node2D>("States/AttackingState");

        _houseFire1 = GetNode<HouseFire>("States/AttackingState/House1Fire/HouseFire");
        _houseFire2 = GetNode<HouseFire>("States/AttackingState/House1Fire/HouseFire2");
        _houseFire3 = GetNode<HouseFire>("States/AttackingState/House1Fire/HouseFire3");
        _oldManHouseFire1 = GetNode<HouseFire>("States/AttackingState/House2Fire/HouseFire");
        _oldManHouseFire2 = GetNode<HouseFire>("States/AttackingState/House2Fire/HouseFire2");
        _oldManHouseFire3 = GetNode<HouseFire>("States/AttackingState/House2Fire/HouseFire3");

        _musicManager.PlayTrack(Tracks.Track.SoftBall);
    }

    public void OnUndergroundCameraTriggered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _room.Enabled = false;
            _room2.Enabled = false;
            _room1Underground.Enabled = true;
            _room2Underground.Enabled = true;
        }
    }

    public void OnOvergroundCameraTriggered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            _room.Enabled = true;
            _room2.Enabled = true;
            _room1Underground.Enabled = false;
            _room2Underground.Enabled = false;
        }
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