using Godot;

namespace Theadventureofhink.autoloads;

public static class CameraLimiter
{
    public static void ApplyCollisionShapeToCameraLimits(Camera2D camera, CollisionShape2D collisionShape)
    {
        var rect = collisionShape.Shape.GetRect();
        rect.Position = collisionShape.GlobalPosition + rect.Position;
        camera.SetLimit(Side.Left, (int)rect.Position.X);
        camera.SetLimit(Side.Right, (int)(rect.Position.X + rect.Size.X));
        camera.SetLimit(Side.Top, (int)rect.Position.Y);
        camera.SetLimit(Side.Bottom, (int)(rect.Position.Y + rect.Size.Y));
    }
}