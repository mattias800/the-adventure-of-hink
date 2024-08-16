namespace Theadventureofhink.game_state;

public class CharactersState
{
    public LittleMushroomState LittleMushroomState { get; set; }  = new();
    public GrandpaState GrandpaState { get; set; }  = new();
    public BlacksmithState BlacksmithState { get; set; }  = new();
}

public class LittleMushroomState
{
    public BooleanState HasEverMet { get; set; }  = new();
}

public class BlacksmithState
{
    public BlacksmithDialogueState DialogueState  { get; set; } = new();
}

public class BlacksmithDialogueState
{
    public BooleanState HasEverMet  { get; set; } = new();
}

public class GrandpaState
{
    public GrandpaDialogueState DialogueState  { get; set; } = new();
}

public class GrandpaDialogueState
{
    public BooleanState HasEverMet  { get; set; } = new();
    public BooleanState HasMentionedGoingBackToForest { get; set; }  = new();
}