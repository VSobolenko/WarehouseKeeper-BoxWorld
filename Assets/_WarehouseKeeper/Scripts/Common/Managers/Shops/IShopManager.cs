using System.Collections.Generic;
using System.Threading.Tasks;

namespace WarehouseKeeper.Shops
{
internal interface IShopManager
{
    Task<bool> Initialize();

    HashSet<GameProduct> Products { get; }
    
    Task<PurchaseResult> PurchaseProduct(string productId);
}

internal enum PurchaseResult : byte
{
    Success,
    Cancel,
    Error,
}
}