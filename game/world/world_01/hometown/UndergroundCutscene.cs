using Godot;
using System;
using System.Linq;
using Theadventureofhink.entities.fire;
using Theadventureofhink.utils;

public partial class UndergroundCutscene : Node2D
{
    public override void _Ready()
    {
        SetAllEnemiesVisible(false);
    }

    public override void _Process(double delta)
    {
        
    }

    public void OnBodyEnteredFireTrigger(Node2D body)
    {
        GD.Print("FIRE");
        if (CollisionUtil.IsPlayer(body))
        {
            var fires = GetTree().GetNodesInGroup("fires").OfType<HouseFire>().ToList();

            GD.Print("found " + fires.Count() + " fires");
            foreach (var fire in fires)
            {
                fire.State = FireState.OnFire;
            }

            SetAllEnemiesVisible(true);
        }
    }

    public void SetAllEnemiesVisible(bool visible)
    {
        var list = GetTree().GetNodesInGroup("enemies").OfType<Node2D>().ToList();
        foreach (var e in list)
        {
            e.Visible = visible;
        }
    }
}