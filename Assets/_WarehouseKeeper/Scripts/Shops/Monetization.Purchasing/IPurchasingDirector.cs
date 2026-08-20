using System;
using System.Threading;
using System.Threading.Tasks;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing
{
internal interface IPurchasingDirector
{
    bool HasActivePurchaseTransaction { get; }
    event Action<string> OnConfirmedProductBuy;

    Task InitializeAsync(CancellationToken token);
    Task<PurchaseRequestResult> PurchaseProduct(string productId, CancellationToken token);
}
}