using Godot;

namespace Theadventureofhink.game_state;

public class PlayerState
{
    public PlayerSkillsState PlayerSkillsState = new();
    public PlayerItemsState PlayerItemsState = new();
    public PlayerStatsState PlayerStatsState = new();
}