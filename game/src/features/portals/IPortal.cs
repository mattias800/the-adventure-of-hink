using Godot;
using Theadventureofhink.world;

namespace Theadventureofhink.entities.portals;

public interface IPortal
{
    public Stage GetNextStage();
    
    public string GetTargetPortalName();

    public string GetName();

    public Vector2 GetSpawnPosition();
}