namespace Theadventureofhink.game_state;

public class CharactersState
{
    public LittleMushroomState LittleMushroomState = new();
}

public class LittleMushroomState
{
    public BooleanState HasEverMet = new();
}