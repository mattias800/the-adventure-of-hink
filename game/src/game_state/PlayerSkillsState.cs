namespace Theadventureofhink.game_state;

public class PlayerSkillsState
{
    public BooleanState CanDoubleJump  { get; set; }  = new();
    public BooleanState CanWallJump  { get; set; }  = new();
    public BooleanState CanClimbWalls  { get; set; }  = new();
    public BooleanState CanDash  { get; set; }  = new();

}