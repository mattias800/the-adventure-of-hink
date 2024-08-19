using Godot;
using System;

public partial class TitleScreenBackground : Node2D
{
    private Sprite2D _cloudsFront;
    private Sprite2D _cloudsMid;

    public override void _Ready()
    {
        _cloudsFront = GetNode<Sprite2D>("CloudsFrontTFc");
        _cloudsMid = GetNode<Sprite2D>("CloudsMidTFc");
    }

    public override void _Process(double delta)
    {
        MoveMidClouds((float)delta * 2.0f);
        MoveFrontClouds((float)delta * 8.0f);
    }

    public void MoveMidClouds(float offset)
    {
        var newPosition = _cloudsMid.RegionRect.Position;
        newPosition.X += offset;
        _cloudsMid.RegionRect = new Rect2(newPosition, _cloudsMid.RegionRect.Size);
    }

    public void MoveFrontClouds(float offset)
    {
        var newPosition = _cloudsFront.RegionRect.Position;
        newPosition.X += offset;
        _cloudsFront.RegionRect = new Rect2(newPosition, _cloudsFront.RegionRect.Size);
    }
}