using Godot;
using System;

public partial class Chest : Node2D
{
	private AnimatedSprite2D _animatedSprite2D;

	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}
	
	public override void _Process(double delta)
	{
	}

	public async void OnInteract()
	{
		_animatedSprite2D.Play("opening");
		await ToSignal(_animatedSprite2D, "animation_looped");
		_animatedSprite2D.Play("open");
	}
}
