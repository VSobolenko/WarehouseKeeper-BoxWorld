using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Purchasing;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP
{
internal interface IUnityIAPProvider<TIn, TOut>
{
    event Action<TIn> OnProductBuy;

    bool IsInitialized { get; }
    bool HasActivePurchaseTransaction { get; }

    Task InitializeAsync(IAPConfigurationData[] products, CancellationToken token);
    Task<TOut> RequestToPurchase(string productId);
    void ConfirmPurchase(TIn product);
}

#if MONETIZATION_PURCHASING_ENABLE_IAP_V4
internal interface IUnityIAPProviderV4 : IUnityIAPProvider<Product, ProductPurchaseRequestResult> { }
#endif

internal interface IUnityIAPProviderV5 : IUnityIAPProvider<PendingOrder, CartPurchaseRequestResult> { }
}