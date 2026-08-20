﻿#if MONETIZATION_PURCHASING_ENABLE_IAP_V4
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

#pragma warning disable 618

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.v4
{
internal class UnityIAPProviderV4 : IDetailedStoreListener, IUnityIAPProviderV4
{
    private readonly UnityIAPLoggerV4 _logger = new();
    private IStoreController _controller;
    private IExtensionProvider _extensions;

    #region Initialize

    private TaskCompletionSource<bool> _initializationCompletionSource;

    public bool IsInitialized => _controller != null && _extensions != null;
    public bool HasActivePurchaseTransaction => _purchaseCompletionSource != null;

    public Task InitializeAsync(IAPConfigurationData[] products, CancellationToken token)
    {
        _logger.LogInitializeSdkAsync();
        _initializationCompletionSource = new TaskCompletionSource<bool>(token);
        var store = StandardPurchasingModule.Instance().appStore;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(store));
        foreach (var product in products)
        {
            builder.AddProduct(product.id, product.productType);
        }

        UnityPurchasing.Initialize(this, builder);

        //CheckForPendingPurchases();
        return _initializationCompletionSource.Task;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        _extensions = extensions;

        _logger.LogOnInitialized();
        _initializationCompletionSource.TrySetResult(true);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        _initializationCompletionSource.TrySetResult(false);
        _logger.LogOnInitializeFailed(error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        _initializationCompletionSource.TrySetResult(false);
        _logger.LogOnInitializeFailed(error, message);
    }

    private void CheckForPendingPurchases()
    {
        if (_controller == null || _controller.products == null)
            return;

        foreach (var product in _controller.products.all)
        {
            if (product.hasReceipt && product.transactionID != null)
            {
                _controller.ConfirmPendingPurchase(product);
            }
        }
    }

    #endregion

    #region Purchase

    private TaskCompletionSource<ProductPurchaseRequestResult> _purchaseCompletionSource;

    public event Action<Product> OnProductBuy;

    public Task<ProductPurchaseRequestResult> RequestToPurchase(string productId)
    {
        if (HasActivePurchaseTransaction)
            throw new InvalidOperationException("Another another transaction is in progress");

        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

        _purchaseCompletionSource = new TaskCompletionSource<ProductPurchaseRequestResult>();
        _logger.LogRequestToPurchase(productId);
        _controller.InitiatePurchase(productId);

        return _purchaseCompletionSource.Task;
    }

    public void ConfirmPurchase(Product product)
    {
        _logger.LogConfirmPurchase(product);
        _controller.ConfirmPendingPurchase(product);
    }

    public virtual PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        _logger.LogProcessPurchase(e.purchasedProduct);
        OnProductBuy?.Invoke(e.purchasedProduct);
        _purchaseCompletionSource?.TrySetResult(new ProductPurchaseRequestResult
        {
            result = PurchaseResult.Success,
            item = e.purchasedProduct,
        });
        _purchaseCompletionSource = null;

        return PurchaseProcessingResult.Pending;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        _logger.LogOnPurchaseFailed(product, failureReason);
        _purchaseCompletionSource?.TrySetResult(new ProductPurchaseRequestResult
        {
            result = failureReason == PurchaseFailureReason.UserCancelled
                ? PurchaseResult.Cancel
                : PurchaseResult.Error,
            reason = failureReason,
            item = product,
        });
        _purchaseCompletionSource = null;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        _logger.LogOnPurchaseFailed(product, failureDescription);
        _purchaseCompletionSource?.TrySetResult(new ProductPurchaseRequestResult
        {
            result = failureDescription.reason == PurchaseFailureReason.UserCancelled
                ? PurchaseResult.Cancel
                : PurchaseResult.Error,
            reason = failureDescription.reason,
            message = failureDescription.message,
            item = product,
        });
        _purchaseCompletionSource = null;
    }

    #endregion

    public string GetCurrentCurrencyCode()
    {
        if (!IsInitialized || _controller.products.all.Length == 0)
        {
            Debug.LogError("IAP not initialized or no products available.");

            return null;
        }

        string currencyCode = _controller.products.all[0].metadata.isoCurrencyCode;

        return currencyCode;
    }
}
#endif
