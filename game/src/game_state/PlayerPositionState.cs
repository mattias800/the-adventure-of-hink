#nullable enable
using Theadventureofhink.world;

namespace Theadventureofhink.game_state;

public class PlayerPositionState
{
    public Stage? LastStage  { get; set; }  = null;
    public string? LastPortalName  { get; set; }  = null;
    public string? LastCheckpointName  { get; set; }  = null;
}