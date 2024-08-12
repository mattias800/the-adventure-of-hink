using Godot;

namespace Theadventureofhink.collectibles.emerald;

public partial class EmeraldSprite : AnimatedSprite2D
{
    public override void _Ready()
    {
        Play("idle");
    }
}