namespace Theadventureofhink.game_state;

public enum CollectibleInstance
{
    EmeraldHometownWesternForestMiddle,
    EmeraldHometownWesternForestTop,
    EmeraldHometownEasterForestTop,
    EmeraldHometownEasterForestRight,
    EmeraldHometownFieldsLeft,
    EmeraldHometownFieldsRight,
    EmeraldHometownUndergroundLeft,
    EmeraldHometownUndergroundBottomRight,
}

public class CollectiblesState
{
    public BooleanState EmeraldHometownWesternForestMiddle = new();
    public BooleanState EmeraldHometownWesternForestTop = new();
    public BooleanState EmeraldHometownEasterForestTop = new();
    public BooleanState EmeraldHometownEasterForestRight = new();
    public BooleanState EmeraldHometownFieldsLeft = new();
    public BooleanState EmeraldHometownFieldsRight = new();
    public BooleanState EmeraldHometownUndergroundLeft = new();
    public BooleanState EmeraldHometownUndergroundBottomRight = new();

    public bool HasBeenCollected(CollectibleInstance collectibleInstance)
    {
        switch (collectibleInstance)
        {
            case CollectibleInstance.EmeraldHometownWesternForestMiddle:
                return EmeraldHometownWesternForestMiddle.Value;
            case CollectibleInstance.EmeraldHometownWesternForestTop:
                return EmeraldHometownWesternForestTop.Value;
            case CollectibleInstance.EmeraldHometownEasterForestTop:
                return EmeraldHometownEasterForestTop.Value;
            case CollectibleInstance.EmeraldHometownEasterForestRight:
                return EmeraldHometownEasterForestRight.Value;
            case CollectibleInstance.EmeraldHometownFieldsLeft:
                return EmeraldHometownFieldsLeft.Value;
            case CollectibleInstance.EmeraldHometownFieldsRight:
                return EmeraldHometownFieldsRight.Value;
            case CollectibleInstance.EmeraldHometownUndergroundLeft:
                return EmeraldHometownUndergroundLeft.Value;
            case CollectibleInstance.EmeraldHometownUndergroundBottomRight:
                return EmeraldHometownUndergroundBottomRight.Value;
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
            case CollectibleInstance.EmeraldHometownFieldsLeft:
                EmeraldHometownFieldsLeft.SetValue(collected);
                return;
            case CollectibleInstance.EmeraldHometownFieldsRight:
                EmeraldHometownFieldsRight.SetValue(collected);
                return;
            case CollectibleInstance.EmeraldHometownUndergroundLeft:
                EmeraldHometownUndergroundLeft.SetValue(collected);
                return;
            case CollectibleInstance.EmeraldHometownUndergroundBottomRight:
                EmeraldHometownUndergroundBottomRight.SetValue(collected);
                return;
        }
    }
}