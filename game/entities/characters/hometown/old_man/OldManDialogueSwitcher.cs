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
		if (GameState.Once(_gameState.CharactersState.GrandpaState.HasEverMet))
		{
			return "grandpa_start";
		}

		return "grandpa_start_repeat";
	}
}
