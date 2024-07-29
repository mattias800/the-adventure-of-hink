using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class OldManDialogueSwitcher : DialogueSwitcher
{
    private GameState _gameState;

    public override void _Ready()
    {
        _gameState = GetNode<GameState>(Singletons.GameState);
    }

    public override string GetDialogueStartPoint()
    {
        if (GameState.Once(_gameState.CharactersState.GrandpaState.DialogueState.HasEverMet))
        {
            return "grandpa_start";
        }

        if (!_gameState.PlayerState.PlayerSkillsState.CanDoubleJump.Value())
        {
            return "grandpa_start_repeat";
        }

        if (GameState.Once(_gameState.CharactersState.GrandpaState.DialogueState.HasMentionedGoingBackToForest))
        {
            return "grandpa_start_suggest_going_to_forest";
        }

        if (!_gameState.PlayerState.PlayerSkillsState.CanClimbWalls.Value())
        {
            return "grandpa_start_suggest_going_to_forest_repeat";
        }

        return "grandpa_reacts_to_wallclimb";
    }
}