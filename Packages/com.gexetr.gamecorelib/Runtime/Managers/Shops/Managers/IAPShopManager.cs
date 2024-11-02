using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

#pragma warning disable 8632

namespace Game.Shops
{
internal class IAPShopManager : IShopManager, IDetailedStoreListener
{
    private readonly GameProduct[] _sourceProducts;
    private TaskCompletionSource<bool> _initializationCompletionSource;
    private TaskCompletionSource<PurchaseResponseResult> _purchaseCompletionSource;
    private IStoreController _controller;
    private IExtensionProvider _extensions;
    public HashSet<GameProduct> Products { get; private set; }

    public IAPShopManager(GameProduct[] sourceProducts)
    {
        _sourceProducts = sourceProducts;
    }

    public async Task<bool> Initialize()
    {
        if (Products != null)
            return await Task.FromResult(true);

        if (_sourceProducts == null)
        {
            Log.Error("Null source products");

            return await Task.FromResult(false);
        }

        var unityServices = new UnityServicesManager();
        await unityServices.Initialize();
        Products = new HashSet<GameProduct>(_sourceProducts.Length);
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var sourceProduct in _sourceProducts)
        {
            if (sourceProduct.Ignored)
                continue;
            builder.AddProduct(sourceProduct.ProductId, sourceProduct.Type);
            Products.Add(sourceProduct);
        }

        _initializationCompletionSource = new TaskCompletionSource<bool>();
        UnityPurchasing.Initialize(this, builder);

        return await _initializationCompletionSource.Task;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        _extensions = extensions;

        if (_initializationCompletionSource?.TrySetResult(true) == false)
            Log.InternalError();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        if (_initializationCompletionSource?.TrySetResult(false) == false)
            Log.Error($"Initialize Failed: {error.ToString()}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string? message)
    {
        if (_initializationCompletionSource?.TrySetResult(false) == false)
            Log.Error($"Initialize Failed: {error.ToString()}; Message: {message}");
    }

    public Task<PurchaseResponseResult> PurchaseProduct(string productId)
    {
        if (_controller == null)
            return Task.FromResult(new PurchaseResponseResult
            {
                result = PurchaseResult.Error,
                message = "Null _controller",
            });

        _purchaseCompletionSource = new TaskCompletionSource<PurchaseResponseResult>();

        try
        {
            var product = Products?.FirstOrDefault(x => x.ProductId == productId);
            if (product == null)
            {
                Log.InternalError();

                return Task.FromResult(new PurchaseResponseResult
                {
                    result = PurchaseResult.Error,
                    message = "Product not found",
                });
            }

            _controller?.InitiatePurchase(productId);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            _purchaseCompletionSource.SetResult(new PurchaseResponseResult
            {
                result = PurchaseResult.Error,
                message = e.Message,
            });
        }

        return _purchaseCompletionSource.Task;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (_purchaseCompletionSource?.TrySetResult(new PurchaseResponseResult
        {
            result = PurchaseResult.Success,
            message = "ProcessPurchase method",
        }) == false)
            Log.InternalError();

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        switch (failureReason)
        {
            case PurchaseFailureReason.UserCancelled:
                if (_purchaseCompletionSource?.TrySetResult(new PurchaseResponseResult
                {
                    result = PurchaseResult.Cancel,
                    message = "OnPurchaseFailed method",
                }) == false)
                    Log.InternalError();

                break;
            default:
                if (_purchaseCompletionSource?.TrySetResult(new PurchaseResponseResult
                {
                    result = PurchaseResult.Error,
                    message = "OnPurchaseFailed method",
                }) == false)
                    Log.InternalError();

                break;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        var purchaseResult = new PurchaseResponseResult
        {
            message = $"ProductId={failureDescription.productId};" +
                      $"Result={failureDescription.reason};" +
                      $"Message={failureDescription.message}",
            
            result = failureDescription.reason switch
            {
                PurchaseFailureReason.UserCancelled => PurchaseResult.Cancel,
                _ => PurchaseResult.Error
            },
        };


        if (_purchaseCompletionSource?.TrySetResult(purchaseResult) == false)
            Log.InternalError();
    }
}
}