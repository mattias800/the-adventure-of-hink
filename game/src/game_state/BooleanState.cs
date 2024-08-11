namespace Theadventureofhink.game_state;

public class BooleanState
{
    private bool _value;

    public BooleanState()
    {
        _value = false;
    }

    public BooleanState(bool value)
    {
        _value = value;
    }

    public bool Value() => _value;

    public void SetValue(bool v)
    {
        _value = v;
    }
}