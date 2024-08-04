using Godot;
using System;
using System.Linq;
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
		_detonator = GetNode<Detonator>("BombLogic/Detonator");
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
		var smokes = GetTree().GetNodesInGroup("bomb_smokes").OfType<CpuParticles2D>();
		foreach (var s in smokes)
		{
			s.Emitting = true;
		}
		_bombBlockGround.QueueFree();
	}
}
