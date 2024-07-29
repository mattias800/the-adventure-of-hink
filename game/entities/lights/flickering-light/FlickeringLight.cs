using Godot;
using System;

public partial class FlickeringLight : PointLight2D
{
	[Export] public float LightStrength = 2.0f;
	
	private FastNoiseLite _noise;

	private int _value;
	private int _maxValue = 1000000;
	
	public override void _Ready()
	{
		_noise = new FastNoiseLite();
		_value = new Random().Next() % 1000000;
	}
	
	public override void _Process(double delta)
	{
		_value ++;
		_value = _value % _maxValue;

		Energy = 2 + _noise.GetNoise1D(_value) * LightStrength;
	}
}
