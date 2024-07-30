using Godot;
using System;
using Theadventureofhink.entities.fire;

public partial class HouseFire : Node2D
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

    private float _deltaSinceStarted = 0.0f;

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
        if (State == FireState.OnFire)
        {
            _deltaSinceStarted += (float)delta;

            if (!_sound.Playing)
            {
                _sound.PitchScale = _pitch;

                // Sound volume tween
                _sound.VolumeDb = -20f;
                var tween = CreateTween();
                tween.SetTrans(Tween.TransitionType.Sine);
                tween.TweenProperty(_sound, "volume_db", 0.0f, 3f);
                _sound.Play();

                _fire.Amount = 0;
                var fireTween = CreateTween();
                fireTween.SetTrans(Tween.TransitionType.Sine);
                fireTween.TweenProperty(_fire, "amount", 16.0f, 3f);
                _sound.Play();

                // Fire amount tween
            }

            _fire.Emitting = State == FireState.OnFire;
            _smoke.Emitting = State is FireState.OnFire or FireState.JustSmoke;
            _light.Enabled = State == FireState.OnFire;

            _value += (int)(delta * 200);
            _value %= _maxValue;

            var power = GetRamp(_deltaSinceStarted, 3.0f);
            _light.Energy = (2 + _noise.GetNoise1D(_value) * LightStrength) * power;
        }
        else
        {
            _deltaSinceStarted = 0;
            if (_sound.Playing)
            {
                _sound.Stop();
            }
        }
    }

    private float GetRamp(float timeLapsed, float timeLapsedForFull)
    {
        return Math.Min(1.0f, timeLapsed / timeLapsedForFull);
    }
}