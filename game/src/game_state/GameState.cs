namespace Theadventureofhink.game_state;

public class GameState
{
    public WorldState WorldState { get; set; } = new();
    public PlayerState PlayerState  { get; set; } = new();
    public CollectiblesState CollectiblesState  { get; set; } = new();
    public PlayerPositionState PlayerPositionState  { get; set; } = new();
    public CharactersState CharactersState  { get; set; } = new();
}