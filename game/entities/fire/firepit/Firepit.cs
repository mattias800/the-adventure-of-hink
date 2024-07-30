using Godot;
using System;
using Theadventureofhink.entities.fire;

public partial class Firepit : Node2D
{
    [Export] public FireState State;
    [Export] public float LightStrength = 2.0f;

    private FastNoiseLite _noise;
    private PointLight2D _light;
    private CpuParticles2D _fire;
    private CpuParticles2D _smoke;
    private AudioStreamPlayer2D _sound;

    private int _value;
    private int _maxValue = 1000000;
    private float _pitch = 1.0f;

    public override void _Ready()
    {
        _noise = new FastNoiseLite();
        _light = GetNode<PointLight2D>("PointLight2D");
        _fire = GetNode<CpuParticles2D>("Fire");
        _smoke = GetNode<CpuParticles2D>("Smoke");
        _sound = GetNode<AudioStreamPlayer2D>("Sound");

        _value = new Random().Next() % 1000000;
        _pitch = new RandomNumberGenerator().RandfRange(0.8f, 1.2f);

        _fire.Emitting = false;
        _smoke.Emitting = false;
        _light.Enabled = false;
    }

    public override void _Process(double delta)
    {
        _fire.Emitting = State == FireState.OnFire;
        _smoke.Emitting = State is FireState.OnFire or FireState.JustSmoke;
        _light.Enabled = State == FireState.OnFire;

        if (State == FireState.OnFire)
        {
            if (!_sound.Playing)
            {
                _sound.PitchScale = _pitch;
                _sound.Play();
            }

            _value++;
            _value %= _maxValue;

            _light.Energy = 2 + _noise.GetNoise1D(_value) * LightStrength;
        }
        else
        {
            if (_sound.Playing)
            {
                _sound.Stop();
            }
        }
    }
}