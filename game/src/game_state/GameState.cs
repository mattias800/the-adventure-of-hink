namespace Theadventureofhink.game_state;

public class GameState
{
    public WorldState WorldState = new();
    public PlayerState PlayerState = new();
    public CollectiblesState CollectiblesState = new();
    public PlayerPositionState PlayerPositionState = new();
    public CharactersState CharactersState = new();
}