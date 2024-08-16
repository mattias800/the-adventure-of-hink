#nullable enable
using Theadventureofhink.world;

namespace Theadventureofhink.game_state;

public class PlayerPositionState
{
    public int LastStage { get; set; } = -1;
    public string LastPortalName { get; set; } = "";
    public string LastCheckpointName { get; set; } = "";
}