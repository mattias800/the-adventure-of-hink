using Godot;
using Theadventureofhink.autoloads;
using Theadventureofhink.utils;

public partial class SawWheel : Node2D
{
	[Export] public float RotationSpeed = 3.0f;

	private GameManager _gameManager;
	
	public override void _Ready()
	{
		_gameManager = GetNode<GameManager>(Singletons.GameManager);
	}
	
	public override void _Process(double delta)
	{
		Rotate(Mathf.Pi * RotationSpeed * (float)delta);
	}

	public void OnArea2dBodyEntered(Node2D body)
	{
		if (CollisionUtil.IsPlayer(body))
		{
			_gameManager.RespawnPlayer();
		}
	}

}
