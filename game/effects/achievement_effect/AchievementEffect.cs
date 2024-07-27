using Godot;

namespace Theadventureofhink.effects.achievement_effect;

public partial class AchievementEffect : Node2D
{
    [Export] public PackedScene AchievementIcon;

    private CpuParticles2D _moreSparkles;
    private AnimatedSprite2D _blueLight;
    private AnimatedSprite2D _sparkles;
    private AudioStreamPlayer2D _sound;

    private bool _running;
    private bool _stopping;
    private float _runCounter;

    public override void _Ready()
    {
        _moreSparkles = GetNode<CpuParticles2D>("MoreSparkles");
        _blueLight = GetNode<AnimatedSprite2D>("BlueLight");
        _sparkles = GetNode<AnimatedSprite2D>("Sparkles");
        _sound = GetNode<AudioStreamPlayer2D>("Sound");
        _moreSparkles.Emitting = false;
        _blueLight.Visible = false;
        _sparkles.Visible = false;
        _running = false;
    }

    public override void _Process(double delta)
    {
    }

    public async void Start()
    {
        if (_running)
        {
            return;
        }

        _running = true;
        _sparkles.Play("default");
        _sparkles.Visible = true;
        _moreSparkles.Emitting = true;
        _sound.Play();
        
        await ToSignal(GetTree().CreateTimer(0.2), Timer.SignalName.Timeout);

        _blueLight.Visible = true;
        _blueLight.Scale = new Vector2(0, 0);
        _blueLight.Play("default");

        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(_blueLight, "scale", new Vector2(0.25f, 0.25f), 0.3);

        var instanceWrapper = new Node2D();
        var instance = AchievementIcon.Instantiate();
        instanceWrapper.Scale = new Vector2(0, 0);
        instanceWrapper.AddChild(instance);
        AddChild(instanceWrapper);
        var thingTween = CreateTween();
        thingTween.SetTrans(Tween.TransitionType.Sine);
        thingTween.TweenProperty(instanceWrapper, "scale", new Vector2(1.0f, 1.0f), 0.3);
    }

    public async void Stop()
    {
        if (_stopping)
        {
            return;
        }

        _stopping = true;
        _moreSparkles.Emitting = false;
        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(_blueLight, "scale", new Vector2(0, 0), 0.3);
        await ToSignal(tween, Tween.SignalName.Finished);
        QueueFree();
    }
}