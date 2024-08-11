using Godot;

namespace Theadventureofhink.features.player.platform;

public partial class WallClimbableProvider : Node2D
{
    public bool IsWallClimbable(RayCast2D ray, float playerVelocityX)
    {
        if (ray.IsColliding())
        {
            var collider = ray.GetCollider();
            if (collider is TileMapLayer)
            {
                var tileMapLayer = collider as TileMapLayer;

                var collisionPoint = ray.GetCollisionPoint();
                // Collision point is just before the tile, so move it into the tile.
                collisionPoint.X += playerVelocityX;

                var tileCoords = tileMapLayer.LocalToMap(collisionPoint);

                var cellTileData = tileMapLayer.GetCellTileData(tileCoords);
                if (cellTileData != null)
                {
                    var customData = cellTileData.GetCustomData("climbable");
                    if (customData.As<bool>() is var climbable)
                    {
                        return climbable;
                    }
                }
            }
        }

        return false;
    }
}