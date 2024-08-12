using Godot;
using System;

public partial class SplashSound : AudioStreamPlayer2D
{
    public override void _Ready()
    {
        Play();
        Finished += QueueFree;
    }
}