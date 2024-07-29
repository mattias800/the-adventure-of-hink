namespace Theadventureofhink.game_state;

public class CharactersState
{
    public LittleMushroomState LittleMushroomState = new();
    public GrandpaState GrandpaState = new();
    public BlacksmithState BlacksmithState = new();
}

public class LittleMushroomState
{
    public BooleanState HasEverMet = new();
}

public class BlacksmithState
{
    public BlacksmithDialogueState DialogueState = new();
}

public class BlacksmithDialogueState
{
    public BooleanState HasEverMet = new();
}

public class GrandpaState
{
    public GrandpaDialogueState DialogueState = new();
}

public class GrandpaDialogueState
{
    public BooleanState HasEverMet = new();
    public BooleanState HasMentionedGoingBackToForest = new();
}