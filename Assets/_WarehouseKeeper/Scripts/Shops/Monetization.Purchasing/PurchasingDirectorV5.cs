using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Purchasing;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.UnityServices;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing
{
internal sealed class PurchasingDirectorV5 : PurchasingDirector<PendingOrder, CartPurchaseRequestResult>
{
    public PurchasingDirectorV5(IAPConfigurationCollection iapCollection, IUnityServicesInitializer unityServices)
        : base(new UnityIAPProviderV5(), iapCollection, unityServices) { }

    protected override async Task TryConfirmUnknownPendingPurchases(PendingOrder order)
    {
        var isPendingOrder = order.CartOrdered.Items()
                                  .All(cartItem => pendingProductsId.Contains(cartItem.Product.definition.id) ==
                                                   false);

        if (isPendingOrder)
            return;

        await FinishPurchaseWithConfirmOnServer(order, default);
    }

    protected override async Task<CartPurchaseRequestResult> FinishPurchaseWithConfirmOnServer(
        PendingOrder order, CancellationToken token)
    {
        var transactionId = order.Info.TransactionID;
        var receipt = order.Info.Receipt;

        var serverConfirmed = await ConfirmPurchaseOnServer(transactionId, receipt, token);

        if (token.IsCancellationRequested)
            return new CartPurchaseRequestResult
            {
                result = PurchaseResult.Cancel,
                message = "Canceled",
            };

        if (serverConfirmed.result != PurchaseResult.Success)
            return serverConfirmed;

        iapProvider.ConfirmPurchase(order);

        foreach (var cartItem in order.CartOrdered.Items())
            InvokeBaseEvent(cartItem.Product.definition.id);

        return serverConfirmed;
    }
}
}