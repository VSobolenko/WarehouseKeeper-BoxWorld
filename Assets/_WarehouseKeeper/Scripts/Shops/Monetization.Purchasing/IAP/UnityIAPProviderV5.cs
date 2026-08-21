using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Purchasing;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP
{
internal sealed class UnityIAPProviderV5 : IUnityIAPProviderV5, IDisposable
{
    private readonly UnityIAPLoggerV5 _logger = new();

    public event Action<PendingOrder> OnProductBuy;
    public bool IsInitialized { get; private set; }
    public bool HasActivePurchaseTransaction => _purchaseCompletionSource != null;

    private StoreController _controller;
    private TaskCompletionSource<CartPurchaseRequestResult> _purchaseCompletionSource;
    private TaskCompletionSource<bool> _initializationCompletionSource;

    public async Task InitializeAsync(IAPConfigurationData[] products, CancellationToken token)
    {
        IsInitialized = false;
        _initializationCompletionSource = new TaskCompletionSource<bool>();

        _controller = UnityIAPServices.StoreController();
        _logger.LogInitializeStart();

        await _controller.Connect();

        if (token.IsCancellationRequested)
        {
            _initializationCompletionSource.TrySetResult(false);
            return;
        }

        _logger.LogOnInitialized();
        _controller.OnProductsFetched += OnProductsFetched;
        _controller.OnPurchasesFetched += OnPurchasesFetched;

        _controller.OnStoreDisconnected += OnStoreDisconnected;
        _controller.OnProductsFetchFailed += OnProductsFetchFailed;
        _controller.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

        _controller.OnPurchaseFailed += OnPurchaseFailed;
        _controller.OnPurchasePending += OnPurchasePending;

        var initialProductsToFetch = new List<ProductDefinition>(products.Length);
        for (var i = 0; i < products.Length; i++)
        {
            var product = products[i];
            initialProductsToFetch.Add(new ProductDefinition(product.id, product.productType));
        }

        _controller.FetchProducts(initialProductsToFetch);
        _logger.LogFetchProducts(initialProductsToFetch);

        // Wait until OnProductsFetched or a fetch-failure fires before returning
        using (token.Register(() => _initializationCompletionSource.TrySetResult(false)))
        {
            await _initializationCompletionSource.Task;
        }
    }

    public Task<CartPurchaseRequestResult> RequestToPurchase(string productId)
    {
        if (HasActivePurchaseTransaction)
            throw new InvalidOperationException("Another another transaction is in progress");

        if (string.IsNullOrEmpty(productId))
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));

        _purchaseCompletionSource = new TaskCompletionSource<CartPurchaseRequestResult>();
        _logger.LogRequestToPurchase(productId);
        _controller.PurchaseProduct(productId);

        return _purchaseCompletionSource.Task;
    }

    public void ConfirmPurchase(PendingOrder order)
    {
        _controller.ConfirmPurchase(order);
        _logger.LogConfirmPurchase(order);
    }

    public void Dispose()
    {
        _controller.OnProductsFetched -= OnProductsFetched;
        _controller.OnPurchasesFetched -= OnPurchasesFetched;

        _controller.OnStoreDisconnected -= OnStoreDisconnected;
        _controller.OnProductsFetchFailed -= OnProductsFetchFailed;
        _controller.OnPurchasesFetchFailed -= OnPurchasesFetchFailed;

        _controller.OnPurchaseFailed -= OnPurchaseFailed;

        _controller.OnPurchasePending -= OnPurchasePending;
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failed)
    {
        IsInitialized = false;
        _logger.LogOnInitializeFailed(failed);
        _initializationCompletionSource?.TrySetResult(false);
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription failed)
    {
        IsInitialized = false;
        _logger.LogOnInitializeFailed(failed);
        _initializationCompletionSource?.TrySetResult(false);
    }

    private void OnProductsFetchFailed(ProductFetchFailed failed)
    {
        IsInitialized = false;
        _logger.LogOnInitializeFailed(failed);
        _initializationCompletionSource?.TrySetResult(false);
    }

    private void OnPurchasePending(PendingOrder order)
    {
        OnProductBuy?.Invoke(order);
        _purchaseCompletionSource?.TrySetResult(new CartPurchaseRequestResult
        {
            result = PurchaseResult.Success,
            item = order,
        });
        _purchaseCompletionSource = null;
        _logger.LogProcessPurchase(order);
    }

    private void OnPurchaseFailed(FailedOrder order)
    {
        _purchaseCompletionSource?.TrySetResult(new CartPurchaseRequestResult
        {
            result = order.FailureReason == PurchaseFailureReason.UserCancelled
                ? PurchaseResult.Cancel
                : PurchaseResult.Error,
            reason = order.FailureReason,
            message = order.Details,
        });
        _purchaseCompletionSource = null;
        _logger.LogOnPurchaseFailed(order);
    }

    private void OnProductsFetched(List<Product> products)
    {
        IsInitialized = true;
        _logger.LogProductsFetched(products);
        _initializationCompletionSource?.TrySetResult(true);

        // Now that products are available, fetch existing purchases (entitlements)
        _controller.FetchPurchases();
    }

    private void OnPurchasesFetched(Orders orders)
    {
        // Process purchases, e.g. check for entitlements from completed orders  
        _logger.LogPurchasesFetched(orders);
    }
}
}