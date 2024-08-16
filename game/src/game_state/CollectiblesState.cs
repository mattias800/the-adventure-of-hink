namespace Theadventureofhink.game_state;

public enum CollectibleInstance
{
    EmeraldHometownWesternForestMiddle,
    EmeraldHometownWesternForestTop,
    EmeraldHometownEasterForestTop,
    EmeraldHometownEasterForestRight,
    EmeraldHometownFieldsLeft,
    EmeraldHometownFieldsRight,
}

public class CollectiblesState
{
    public BooleanState EmeraldHometownWesternForestMiddle  { get; set; } = new();
    public BooleanState EmeraldHometownWesternForestTop  { get; set; } = new();
    public BooleanState EmeraldHometownEasterForestTop  { get; set; } = new();
    public BooleanState EmeraldHometownEasterForestRight  { get; set; } = new();
    public BooleanState EmeraldHometownFieldsLeft  { get; set; } = new();
    public BooleanState EmeraldHometownFieldsRight  { get; set; } = new();

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
        }
    }
}