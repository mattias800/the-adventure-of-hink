namespace Theadventureofhink.game_state;

public class CharactersState
{
    public LittleMushroomState LittleMushroomState = new();
    public GrandpaState GrandpaState = new();
}

public class LittleMushroomState
{
    public BooleanState HasEverMet = new();
}

public class GrandpaState
{
    public BooleanState HasEverMet = new();
}