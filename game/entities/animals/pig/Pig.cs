using Godot;
using System;
using Theadventureofhink.utils;

public partial class Pig : AnimatedSprite2D
{

	private float _animationSpeed = RandomHelper.RandfRange(0.7f, 1.3f);
	
	public override void _Ready()
	{
		Play("idle", _animationSpeed);
	}
	
}
