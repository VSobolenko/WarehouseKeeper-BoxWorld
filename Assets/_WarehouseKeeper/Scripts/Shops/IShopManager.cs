using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Shops
{
#if MONETIZATION_PURCHASING_ENABLE_IAP_V4
public interface IShopManager
{
    Task<bool> Initialize();

    HashSet<GameProduct> Products { get; }
    
    Task<PurchaseResponseResult> PurchaseProduct(string productId);
}
        #endif

public enum PurchaseResult : byte
{
    Success,
    Cancel,
    Error,
}
}
