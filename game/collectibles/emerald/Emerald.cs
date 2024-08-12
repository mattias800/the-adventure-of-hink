using Godot;
using System;

public partial class Emerald : AnimatedSprite2D
{
    public override void _Ready()
    {
        Play("idle");
    }
}