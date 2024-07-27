using Godot;

namespace Theadventureofhink.game_state;

public partial class GameState : Node
{
    public WorldState WorldState = new();
    public PlayerState PlayerState = new();
    public PlayerPositionState PlayerPositionState = new();

    public static bool Once(BooleanState s)
    {
        if (!s.Value())
        {
            return false;
        }

        s.SetValue(true);
        return true;
    }
}