namespace Theadventureofhink.game_state;

public enum CollectibleInstance
{
    EmeraldHometownWesternForestMiddle,
    EmeraldHometownWesternForestTop,
    EmeraldHometownEasterForestTop,
    EmeraldHometownEasterForestRight,
}

public class CollectiblesState
{
    public BooleanState EmeraldHometownWesternForestMiddle = new();
    public BooleanState EmeraldHometownWesternForestTop = new();
    public BooleanState EmeraldHometownEasterForestTop = new();
    public BooleanState EmeraldHometownEasterForestRight = new();

    public bool HasBeenCollected(CollectibleInstance collectibleInstance)
    {
        switch (collectibleInstance)
        {
            case CollectibleInstance.EmeraldHometownWesternForestMiddle:
                return EmeraldHometownWesternForestMiddle.Value();
            case CollectibleInstance.EmeraldHometownWesternForestTop:
                return EmeraldHometownWesternForestTop.Value();
            case CollectibleInstance.EmeraldHometownEasterForestTop:
                return EmeraldHometownEasterForestTop.Value();
            case CollectibleInstance.EmeraldHometownEasterForestRight:
                return EmeraldHometownEasterForestRight.Value();
        }

        return false;
    }

    public void SetCollected(CollectibleInstance collectibleInstance, bool collected = true)
    {
        switch (collectibleInstance)
        {
            case CollectibleInstance.EmeraldHometownWesternForestMiddle:
                EmeraldHometownWesternForestMiddle.SetValue(collected);
                return;
            case CollectibleInstance.EmeraldHometownWesternForestTop:
                EmeraldHometownWesternForestTop.SetValue(collected);
                return;
            case CollectibleInstance.EmeraldHometownEasterForestTop:
                EmeraldHometownEasterForestTop.SetValue(collected);
                return;
            case CollectibleInstance.EmeraldHometownEasterForestRight:
                EmeraldHometownEasterForestRight.SetValue(collected);
                return;
        }
    }
}