using System.Collections.Generic;

namespace Theadventureofhink.world;

public enum Stage
{
    Overworld,
    Hometown,
    HometownOldManHouseInterior,
    HometownWesternForest,
    HometownEasternForest,
    HometownFields,
    HometownUnderground,
    WesternWindMill,
    WesternWindMillInterior,
    TestStage
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
            new StageInfo("Hometown western forest", "res://world/world_01/center/hometown-western-forest/level_01.tscn")
        },
        {
            Stage.HometownEasternForest,
            new StageInfo("Hometown eastern forest", "res://world/world_01/center/hometown-easter-forest/hometown_easter_forest.tscn")
        },
        {
            Stage.HometownFields,
            new StageInfo("Hometown fields", "res://world/world_01/center/hometown-fields/hometown_fields.tscn")
        },
        {
            Stage.HometownUnderground,
            new StageInfo("Hometown underground", "res://world/world_01/underground_01/underground_01.tscn")
        },
        {
            Stage.WesternWindMill,
            new StageInfo("Hometown underground", "res://world/world_01/left/western_windmill/western_windmill.tscn")
        },
        {
            Stage.WesternWindMillInterior,
            new StageInfo("Hometown underground", "res://world/world_01/left/western_windmill/western_windmill_indoors.tscn")
        },
        {
            Stage.TestStage,
            new StageInfo("Hometown eastern forest", "res://world/world_01/level_02/level_02.tscn")
        }
    };

    public static StageInfo GetStateInfo(Stage stage)
    {
        return _stageInfos[stage];
    }
}

public record StageInfo(string Name, string FilePath);