using UnityEngine.Purchasing;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing
{
internal enum PurchaseResult : byte
{
    Success,
    Cancel,
    Error,
}

internal class PurchaseRequestResult
{
    public PurchaseResult result;
    public string message;
}

internal class StorePurchaseRequestResult<T> : PurchaseRequestResult
{
    public PurchaseFailureReason reason;
    public T item;
}

internal sealed class ProductPurchaseRequestResult : StorePurchaseRequestResult<Product> { }

internal sealed class CartPurchaseRequestResult : StorePurchaseRequestResult<PendingOrder> { }
}