using Godot;

namespace Theadventureofhink.game_state;

public partial class GameState : Node
{
    public WorldState WorldState = new();
    public PlayerState PlayerState = new();
    public CollectiblesState CollectiblesState = new();
    public PlayerPositionState PlayerPositionState = new();
    public CharactersState CharactersState = new();

    public static bool Once(BooleanState s)
    {
        if (!s.Value())
        {
            s.SetValue(true);
            return true;
        }
        
        return false;
    }
}