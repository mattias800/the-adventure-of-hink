using Godot;
using System;

public partial class WalljumpChest : Node2D
{
    [Export] public PackedScene Achievement;

    private CanvasLayer _canvasLayer;


    public override void _Ready()
    {
        _canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
    }

    public override void _Process(double delta)
    {
    }

    public void OnOpen()
    {
        GD.Print("OnOpen cheeesty");
        _canvasLayer.AddChild(Achievement.Instantiate());
    }
}