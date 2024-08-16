using Theadventureofhink.autoloads;
using Theadventureofhink.game_state;

public partial class OldManDialogueSwitcher : DialogueSwitcher
{
    private GameStateManager _gameStateManager;

    public override void _Ready()
    {
        _gameStateManager = GetNode<GameStateManager>(Singletons.GameStateManager);
    }

    public override string GetDialogueStartPoint()
    {
        if (GameStateManager.Once(_gameStateManager.GameState.CharactersState.GrandpaState.DialogueState.HasEverMet))
        {
            return "grandpa_start";
        }

        if (!_gameStateManager.GameState.PlayerState.PlayerSkillsState.CanDoubleJump.Value)
        {
            return "grandpa_start_repeat";
        }

        if (GameStateManager.Once(_gameStateManager.GameState.CharactersState.GrandpaState.DialogueState.HasMentionedGoingBackToForest))
        {
            return "grandpa_start_suggest_going_to_forest";
        }

        if (!_gameStateManager.GameState.PlayerState.PlayerSkillsState.CanClimbWalls.Value)
        {
            return "grandpa_start_suggest_going_to_forest_repeat";
        }

        return "grandpa_reacts_to_wallclimb";
    }
}