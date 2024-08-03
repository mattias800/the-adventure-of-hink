using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class Underground01 : Node2D
{
	private GameState _gameState;
	private Bomb _bomb;
	
	public override void _Ready()
	{
		_gameState = GetNode<GameState>(Singletons.GameState);
		_bomb = GetNode<Bomb>("Bomb");
		
		// Prevent player from getting stuck, if this was reached too early for some reason.
		_gameState.PlayerState.PlayerSkillsState.CanDoubleJump.SetValue(true);
		_gameState.PlayerState.PlayerSkillsState.CanWallJump.SetValue(true);
		_gameState.PlayerState.PlayerSkillsState.CanClimbWalls.SetValue(true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.O))
		{
			_bomb.LightFuse();
		}
	}
}
