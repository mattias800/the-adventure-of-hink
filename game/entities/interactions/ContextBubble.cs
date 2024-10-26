using Godot;

public partial class ContextBubble : Node2D
{
    private Label _label;
    private MarginContainer _marginContainer;

    public override void _Ready()
    {
        _marginContainer = GetNode<MarginContainer>("MarginContainer");
        _label = GetNode<Label>("%Label");
    }

    public float GetWidth()
    {
        return _marginContainer.Size.X;
    }

    public void SetLabel(string s)
    {
        _label.Text = s;
    }
}