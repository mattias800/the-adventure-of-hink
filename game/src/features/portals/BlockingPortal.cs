using Godot;
using Theadventureofhink.entities.portals;
using Theadventureofhink.utils;
using Theadventureofhink.world;

public partial class BlockingPortal : StaticBody2D, IPortal
{
    [Export] public bool Enabled = true;

    [Export] public Stage NextStage;

    [Export] public string TargetPortalName;
    
    public override void _Ready()
    {
    }
    
    public Stage GetNextStage()
    {
        return NextStage;
    }

    public string GetTargetPortalName()
    {
        return TargetPortalName;
    }

    public new string GetName()
    {
        return Name;
    }

    public Vector2 GetSpawnPosition()
    {
        return GlobalPosition;
    }
}