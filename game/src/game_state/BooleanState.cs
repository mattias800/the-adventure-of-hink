namespace Theadventureofhink.game_state;

public class BooleanState
{
    public bool Value;

    public BooleanState()
    {
        Value = false;
    }

    public BooleanState(bool value)
    {
        Value = value;
    }

    public void SetValue(bool v)
    {
        Value = v;
    }
}