using Godot;

namespace Theadventureofhink.game_state;

public partial class GameStateManager : Node
{

    public GameState GameState = new();
    
    public static bool Once(BooleanState s)
    {
        if (!s.Value)
        {
            s.SetValue(true);
            return true;
        }
        
        return false;
    }
    
    public void IncreaseNumberOfPlayerDeaths()
    {
        GameState.PlayerState.PlayerStatsState.NumberOfDeaths++;
    }
}