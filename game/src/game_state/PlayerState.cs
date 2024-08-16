using Godot;

namespace Theadventureofhink.game_state;

public class PlayerState
{
    public PlayerSkillsState PlayerSkillsState { get; set; }  = new();
    public PlayerItemsState PlayerItemsState { get; set; }  = new();
    public PlayerStatsState PlayerStatsState { get; set; }  = new();
}