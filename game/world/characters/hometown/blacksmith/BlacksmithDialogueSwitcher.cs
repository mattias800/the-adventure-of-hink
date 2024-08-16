using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class BlacksmithDialogueSwitcher : DialogueSwitcher
{
    private GameStateManager _gameStateManager;

    public override void _Ready()
    {
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
    }

    public override string GetDialogueStartPoint()
    {
        if (GameStateManager.Once(_gameStateManager.GameState.CharactersState.BlacksmithState.DialogueState.HasEverMet))
        {
            return "blacksmith_start";
        }

        if (!_gameStateManager.GameState.PlayerState.PlayerSkillsState.CanDoubleJump.Value)
        {
            return "blacksmith_has_no_double_jump";
        }

        if (!_gameStateManager.GameState.PlayerState.PlayerSkillsState.CanClimbWalls.Value)
        {
            return "blacksmith_suggest_going_to_forest";
        }

        return "blacksmith_when_has_wall_climb";
    }
}