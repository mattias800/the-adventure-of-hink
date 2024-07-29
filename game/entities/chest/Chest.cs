using Godot;

namespace Theadventureofhink.entities.chest;

public partial class Chest : Node2D
{
    [Signal]
    public delegate void ChestOpenedEventHandler();

    [Export] public bool Flipped;
    [Export] public bool IsOpen;

    private AnimatedSprite2D _animatedSprite2D;
    private AudioStreamPlayer2D _openSound;
    private AudioStreamPlayer2D _closeSound;

    private Interactible _interactible;

    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _openSound = GetNode<AudioStreamPlayer2D>("OpenSound");
        _closeSound = GetNode<AudioStreamPlayer2D>("CloseSound");
        _interactible = GetNode<Interactible>("Interactible");
    }

    public override void _Process(double delta)
    {
        if (IsOpen)
        {
            _animatedSprite2D.Play("open");
            _interactible.IsActive = false;
        }

        _animatedSprite2D.FlipH = Flipped;
    }

    public async void OnInteract()
    {
        _animatedSprite2D.Play("opening");
        _openSound.Play();
        await ToSignal(_animatedSprite2D, "animation_looped");
        _animatedSprite2D.Play("open");
        EmitSignal(SignalName.ChestOpened);
    }
}