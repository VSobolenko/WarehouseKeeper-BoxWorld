using System;
using System.Linq;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Shops;

namespace WarehouseKeeper.Directors.UI.Shops
{
internal class ShopDirector
{
    private readonly PlayerResourcesDirector _playerResourcesDirector;
    public LocalGameProduct[] LocalProducts { get; }

    public ShopDirector(ResourcesDirector resourcesDirector, PlayerResourcesDirector playerResourcesDirector)
    {
        _playerResourcesDirector = playerResourcesDirector;
        LocalProducts = resourcesDirector.ShopProducts.LocalGameProducts;
    }

    public bool CanBuyProduct(string productId)
    {
        var product = LocalProducts.FirstOrDefault(x => x.ProductId == productId);
        if (product==null)
            return false;

        foreach (var reward in product.Rewards)
        {
            switch (reward.type)
            {
                case RewardType.Hint:
                    if (_playerResourcesDirector.UserData.Amber.CanSpend((int) product.Price))
                        return true;
                    break;
                default:
                    return false;
            }
        }

        return false;
    }
}
}