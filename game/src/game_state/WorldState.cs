using Theadventureofhink.game_state.world;

namespace Theadventureofhink.game_state;

public class WorldState
{
    public Overworld01State Overworld01State = new();
    public Level01State Level01State = new();
    public HometownState HometownState = new();
}