using Godot;
using System;
using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class Underground01 : Node2D
{
	private GameState _gameState;
	private Detonator _detonator;
	private TileMapLayer _bombBlockGround;
	
	public override void _Ready()
	{
		_gameState = GetNode<GameState>(Singletons.GameState);
		_detonator = GetNode<Detonator>("Detonator");
		_bombBlockGround = GetNode<TileMapLayer>("BombBlockGround");
		
		// Prevent player from getting stuck, if this was reached too early for some reason.
		_gameState.PlayerState.PlayerSkillsState.CanDoubleJump.SetValue(true);
		_gameState.PlayerState.PlayerSkillsState.CanWallJump.SetValue(true);
		_gameState.PlayerState.PlayerSkillsState.CanClimbWalls.SetValue(true);
	}
	
	public void OnPlayerInteractWithBomb()
	{
		GD.Print("Interact!");
		_detonator.Arm();
	}
	
	public void OnBombExplode()
	{
		_bombBlockGround.QueueFree();
	}
}
