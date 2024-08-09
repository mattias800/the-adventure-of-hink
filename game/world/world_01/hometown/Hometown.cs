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

	private Theadventureofhink.autoloads.MusicManager _musicManager;
	private GameState _gameState;

	public override void _Ready()
	{
		_musicManager = GetNode<Theadventureofhink.autoloads.MusicManager>(Singletons.MusicManager);
		_gameState = GetNode<GameState>(Singletons.GameState);

		_room = GetNode<Room>("Rooms/Room");
		_room2 = GetNode<Room>("Rooms/Room2");
		_room1Underground = GetNode<Room>("Rooms/Room1Underground");
		_room2Underground = GetNode<Room>("Rooms/Room2Underground");
		_attackingState = GetNode<Node2D>("States/AttackingState");

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
}
