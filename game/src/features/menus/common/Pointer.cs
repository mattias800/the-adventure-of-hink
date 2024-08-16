using Godot;
using System;

public partial class Pointer : Node2D
{
    private AudioStreamPlayer _moveSound;
    private AudioStreamPlayer _acceptSound;

    public override void _Ready()
    {
        _moveSound = GetNode<AudioStreamPlayer>("MoveSound");
        _acceptSound = GetNode<AudioStreamPlayer>("AcceptSound");
    }

    public void PointAt(Control target)
    {
        GlobalPosition = target.GlobalPosition + new Vector2(-50, 10);
    }

    public void PointAtAndPlaySound(Control target)
    {
        PointAt(target);
        GlobalPosition = target.GlobalPosition + new Vector2(0, 0);
        _moveSound.Play();
    }
}