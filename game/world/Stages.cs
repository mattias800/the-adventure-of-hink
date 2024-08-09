using System.Collections.Generic;

namespace Theadventureofhink.world;

public enum Stage
{
    Overworld,
    Hometown,
    HometownOldManHouseInterior,
    HometownWesternForest,
    HometownEasternForest,
    HometownUnderground,
}

public static class Stages
{
    private static Dictionary<Stage, StageInfo> _stageInfos = new()
    {
        { Stage.Overworld, new StageInfo("Overworld", "res://world/overworld_01/overworld_01.tscn") },
        { Stage.Hometown, new StageInfo("Hometown", "res://world/world_01/hometown/hometown.tscn") },
        {
            Stage.HometownOldManHouseInterior,
            new StageInfo("Hometown old man house", "res://world/world_01/hometown/houses/old_man_house.tscn")
        },
        {
            Stage.HometownWesternForest,
            new StageInfo("Hometown western forest", "res://world/world_01/level_01/level_01.tscn")
        },
        {
            Stage.HometownEasternForest,
            new StageInfo("Hometown eastern forest", "res://world/world_01/level_02/level_02.tscn")
        },
        {
            Stage.HometownUnderground,
            new StageInfo("Hometown underground", "res://world/world_01/underground_01/underground_01.tscn")
        }
    };

    public static StageInfo GetStateInfo(Stage stage)
    {
        return _stageInfos[stage];
    }
}

public record StageInfo(string Name, string FilePath);