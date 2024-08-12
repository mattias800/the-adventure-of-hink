using Theadventureofhink.game_state.world;

namespace Theadventureofhink.game_state;

public class WorldState
{
    public Overworld01State Overworld01State = new();
    public HometownWesternForestState HometownWesternForestState = new();
    public HometownState HometownState = new();
}