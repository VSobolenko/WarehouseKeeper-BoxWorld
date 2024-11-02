using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Shops
{
public interface IShopManager
{
    Task<bool> Initialize();

    HashSet<GameProduct> Products { get; }
    
    Task<PurchaseResponseResult> PurchaseProduct(string productId);
}

public enum PurchaseResult : byte
{
    Success,
    Cancel,
    Error,
}
}