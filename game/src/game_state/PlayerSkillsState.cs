namespace Theadventureofhink.game_state;

public class PlayerSkillsState
{
    public BooleanState CanDoubleJump = new();
    public BooleanState CanWallJump = new();
    public BooleanState CanClimbWalls = new();
    public BooleanState CanDash = new();
}