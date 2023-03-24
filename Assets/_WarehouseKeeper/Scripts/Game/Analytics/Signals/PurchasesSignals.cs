using WarehouseKeeper.Shops;

namespace WarehouseKeeper.Directors.Game.Analytics.Signals
{
internal struct PurchaseAmber
{
    public string productId;
    public PurchaseResult result;
}

internal struct PurchaseProduct
{
    public string productId;
    public string place;
    public int amberInitValue;
    public int hintInitValue;
    public ShopProductReward reward;
}
}