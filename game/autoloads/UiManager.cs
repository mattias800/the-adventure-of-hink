using Godot;
using System;

public partial class UiManager : Node2D
{

	private CanvasLayer _canvasLayer;
	private Node2D _center;
	
	
	public override void _Ready()
	{
		_canvasLayer = GetNode<CanvasLayer>("CanvasLayer");
		_center = GetNode<Node2D>("CanvasLayer/Center");
	}

	
	public override void _Process(double delta)
	{
	}

	public void AddUiChild(Node node)
	{
		_canvasLayer.AddChild(node);
	}

	public void AddUiChildCentered(Node node)
	{
		_center.AddChild(node);
	}
}
