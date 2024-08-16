using Theadventureofhink.game_state.world;

namespace Theadventureofhink.game_state;

public class WorldState
{
    public Overworld01State Overworld01State  { get; set; } = new();
    public HometownWesternForestState HometownWesternForestState  { get; set; } = new();
    public HometownState HometownState  { get; set; } = new();
}