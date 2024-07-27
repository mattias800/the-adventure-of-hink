using Godot;

public partial class AchievementEffect : Node2D
{

	private AnimatedSprite2D _blueLight;
	private AnimatedSprite2D _sparkles;
	
	private bool _running;
	private bool _stopping;
	private float _runCounter;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_blueLight = GetNode<AnimatedSprite2D>("BlueLight");
		_sparkles = GetNode<AnimatedSprite2D>("Sparkles");
		_blueLight.Visible = false;
		_sparkles.Visible = false;
		_running = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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

		await ToSignal(GetTree().CreateTimer(0.2), "timeout");

		_blueLight.Visible = true;
		_blueLight.Scale = new Vector2(0, 0);
		_blueLight.Play("default");

		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(_blueLight, "scale", new Vector2(0.25f, 0.25f), 0.3);
	}

	public async void Stop()
	{
		if (_stopping)
		{
			return;
		}

		_stopping = true;
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(_blueLight, "scale", new Vector2(0, 0), 0.3);
		await ToSignal(tween, "finished");
		QueueFree();
	}
}