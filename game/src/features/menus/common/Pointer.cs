using Godot;

public partial class Pointer : Node2D
{
    private AudioStreamPlayer _moveSound;
    private AudioStreamPlayer _acceptSound;

    public override void _Ready()
    {
        _moveSound = GetNode<AudioStreamPlayer>("MoveSound");
        _acceptSound = GetNode<AudioStreamPlayer>("AcceptSound");
    }

    public void PointAtAndPlaySound(Control target)
    {
        GlobalPosition = target.GlobalPosition;
        _moveSound.Play();
    }

    public void PointAtAndPlaySound(Node2D target)
    {
        GlobalPosition = target.GlobalPosition;
        _moveSound.Play();
    }
    
    public void PointAtAndPlaySound(Vector2 target)
    {
        GlobalPosition = target;
        _moveSound.Play();
    }
}