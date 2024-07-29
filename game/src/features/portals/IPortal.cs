using Godot;

namespace Theadventureofhink.entities.portals;

public interface IPortal
{
    public string? GetNextScenePath();
    public string GetTargetPortalName();

    public string GetName();

    public Vector2 GetSpawnPosition();
}