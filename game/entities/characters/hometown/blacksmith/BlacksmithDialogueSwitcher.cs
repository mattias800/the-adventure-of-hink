using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class BlacksmithDialogueSwitcher : DialogueSwitcher
{
    private GameState _gameState;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>(Singletons.GameState);
    }

    public override string GetDialogueStartPoint()
    {
        if (GameState.Once(_gameState.CharactersState.BlacksmithState.DialogueState.HasEverMet))
        {
            return "blacksmith_start";
        }

        if (!_gameState.PlayerState.PlayerSkillsState.CanDoubleJump.Value())
        {
            return "blacksmith_has_no_double_jump";
        }

        if (!_gameState.PlayerState.PlayerSkillsState.CanClimbWalls.Value())
        {
            return "blacksmith_suggest_going_to_forest";
        }

        return "blacksmith_when_has_wall_climb";
    }
}