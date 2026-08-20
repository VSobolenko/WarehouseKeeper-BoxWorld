#if MONETIZATION_PURCHASING_ENABLE_IAP_V4
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Purchasing;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.UnityServices;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.v4
{
internal sealed class PurchasingDirectorV4 : PurchasingDirector<Product, ProductPurchaseRequestResult>
{
    public PurchasingDirectorV4(IAPConfigurationCollection iapCollection, IUnityServicesInitializer unityServices)
        : base(new UnityIAPProviderV4(), iapCollection, unityServices) { }

    protected override async Task TryConfirmUnknownPendingPurchases(Product item)
    {
        if (pendingProductsId.Contains(item.definition.id))
            return;

        await FinishPurchaseWithConfirmOnServer(item, default);
    }

    protected override async Task<ProductPurchaseRequestResult> FinishPurchaseWithConfirmOnServer(
        Product item, CancellationToken token)
    {
#pragma warning disable 618
        var transactionId = item.transactionID;
        var receipt = item.receipt;
#pragma warning restore 618

        var serverConfirmed = await ConfirmPurchaseOnServer(transactionId, receipt, token);

        if (token.IsCancellationRequested)
            return new ProductPurchaseRequestResult
            {
                result = PurchaseResult.Cancel,
                message = "Canceled",
            };

        if (serverConfirmed.result != PurchaseResult.Success)
            return serverConfirmed;

        iapProvider.ConfirmPurchase(item);
        InvokeBaseEvent(item.definition.id);

        return serverConfirmed;
    }
}
#endif
