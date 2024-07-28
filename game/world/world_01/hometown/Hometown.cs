using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;
using Theadventureofhink.utils;

public partial class Hometown : Node2D
{
    private Room room;
    private Room room_2;
    private Room room_1_underground;
    private Room room_2_underground;
    private TileMapLayer ground_over_well_hole;
    private Node2D attacking_state;

    private MusicManager _musicManager;
    private GameState _gameState;

    public override void _Ready()
    {
        _musicManager = GetNode<MusicManager>(Singletons.MusicManager);
        _gameState = GetNode<GameState>(Singletons.GameState);

        room = GetNode<Room>("Rooms/Room");
        room_2 = GetNode<Room>("Rooms/Room2");
        room_1_underground = GetNode<Room>("Rooms/Room1Underground");
        room_2_underground = GetNode<Room>("Rooms/Room2Underground");
        ground_over_well_hole = GetNode<TileMapLayer>("GroundOverWellHole");
        attacking_state = GetNode<Node2D>("AttackingState");

        _musicManager.PlayTrack(Tracks.Track.SoftBall);
        if (!_gameState.WorldState.HometownState.IsUnderAttack.Value())
        {
            attacking_state.QueueFree();
        }
    }

    public void OnUndergroundCameraTriggered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            room.Enabled = false;
            room_2.Enabled = false;
            room_1_underground.Enabled = true;
            room_2_underground.Enabled = true;
        }
    }

    public void OnOvergroundCameraTriggered(Node2D body)
    {
        if (CollisionUtil.IsPlayer(body))
        {
            room.Enabled = true;
            room_2.Enabled = true;
            room_1_underground.Enabled = false;
            room_2_underground.Enabled = false;
        }
    }

    public void OnPlayerEnteredWell()
    {
        ground_over_well_hole.Visible = false;
    }
}