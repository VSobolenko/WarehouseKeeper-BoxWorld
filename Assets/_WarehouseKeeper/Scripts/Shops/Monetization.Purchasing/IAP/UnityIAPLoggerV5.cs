using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP
{
internal sealed class UnityIAPLoggerV5
{
    public void LogOnInitializeFailed(PurchasesFetchFailureDescription failed) { }

    public void LogOnInitializeFailed(StoreConnectionFailureDescription failed) { }

    public void LogOnInitializeFailed(ProductFetchFailed failed) { }

    public void LogProcessPurchase(PendingOrder order) { }

    public void LogOnPurchaseFailed(FailedOrder order) { }

    public void LogProductsFetched(List<Product> products) { }

    public void LogPurchasesFetched(Orders orders) { }

    public void LogConfirmPurchase(PendingOrder order) { }

    public void LogRequestToPurchase(string productId) { }

    public void LogInitializeStart() { }

    public void LogOnInitialized() { }

    public void LogFetchProducts(List<ProductDefinition> initialProductsToFetch) { }
}
}