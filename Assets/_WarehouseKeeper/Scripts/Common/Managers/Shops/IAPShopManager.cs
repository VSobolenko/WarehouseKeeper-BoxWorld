using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Purchasing;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Shops
{
internal class IAPShopManager : IShopManager, IStoreListener
{
    private readonly GameProduct[] _sourceProducts;
    private TaskCompletionSource<bool> _initializationCompletionSource;
    private TaskCompletionSource<PurchaseResult> _purchaseCompletionSource;
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
            Log.WriteError("Null source products");
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
            Log.WriteError($"Initialize Failed: {error.ToString()}");
    }
    
    public Task<PurchaseResult> PurchaseProduct(string productId)
    {
        _purchaseCompletionSource = new TaskCompletionSource<PurchaseResult>();

        try
        {
            var product = Products?.FirstOrDefault(x => x.ProductId == productId);
            if (product == null)
            {
                Log.InternalError();
                return Task.FromResult(PurchaseResult.Error);
            }
            _controller?.InitiatePurchase(productId);
        }
        catch (Exception e)
        {
            Log.WriteError(e.Message);
            _purchaseCompletionSource.SetResult(PurchaseResult.Error);
        }

        return _purchaseCompletionSource.Task;

    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (_purchaseCompletionSource?.TrySetResult(PurchaseResult.Success) == false)
            Log.InternalError();

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        switch (failureReason)
        {
            case PurchaseFailureReason.UserCancelled:
                if (_purchaseCompletionSource?.TrySetResult(PurchaseResult.Cancel) == false)
                    Log.InternalError();
                break;
            default:
                if (_purchaseCompletionSource?.TrySetResult(PurchaseResult.Error) == false)
                    Log.InternalError();
                break;
        }
    }
}
}