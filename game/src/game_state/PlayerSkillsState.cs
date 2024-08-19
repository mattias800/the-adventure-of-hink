namespace Theadventureofhink.game_state;

public class PlayerSkillsState
{
    public BooleanState CanDoubleJump = new(true);
    public BooleanState CanWallJump = new(true);
    public BooleanState CanClimbWalls = new(true);
    public BooleanState CanDash = new();
}