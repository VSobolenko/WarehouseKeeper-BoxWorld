using Game.Shops;

namespace WarehouseKeeper.Directors.Game.Analytics.Signals
{
internal struct ShopEvent
{
    public string message;
    public string time;
}

internal struct PurchaseAmber
{
    public string productId;
    public PurchaseResult result;
    public string message;
    public string time;
}

internal struct PurchaseProduct
{
    public string productId;
    public string place;
    public int amberInitValue;
    public int hintInitValue;
    public ShopProductReward reward;
    public string time;
}
}