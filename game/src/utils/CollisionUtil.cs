using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Theadventureofhink.utils;

public static class CollisionUtil
{
    public static bool BodiesContainPlayer(IEnumerable<Node> bodies)
    {
        return bodies.Any(IsPlayer);
    }

    public static IEnumerable<Node> BodiesExceptPlayer(IList<Node> bodies)
    {
        return bodies.Where(b => !IsPlayer(b));
    }

    public static bool BodiesContainOnlyPlayer(IList<Node> bodies)
    {
        return bodies.Count == 1 && IsPlayer(bodies[0]);
    }

    public static bool IsPlayer(Node body) => body.IsInGroup("player");
}