using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Extensions;
using UnityEngine;
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
    private bool _isInitializedInProcess;
    public HashSet<GameProduct> Products { get; private set; }

    private int _initStart;
    private int _unityServicesManagerComplete;
    private int _unityPurchasingStart;
    private int _initializedComplete;
    private int _initializedFailed;
    private InitializationFailureReason _initializedFailedError;
    private string _initializedFailedMessage = string.Empty;

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

        _isInitializedInProcess = true;
        _initStart++;
        await InitializeUnityServices();
        InitializeUnityPurchasing();

        return await _initializationCompletionSource.Task;
    }

    private void InitializeUnityPurchasing()
    {
        Products = new HashSet<GameProduct>(_sourceProducts.Length);
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var sourceProduct in _sourceProducts)
        {
            if (sourceProduct.Ignored)
                continue;
            builder.AddProduct(sourceProduct.ProductId, (UnityEngine.Purchasing.ProductType) sourceProduct.Type);
            Products.Add(sourceProduct);
        }

        _initializationCompletionSource = new TaskCompletionSource<bool>();
        UnityPurchasing.Initialize(this, builder);
        _unityPurchasingStart++;
    }

    private async Task InitializeUnityServices()
    {
        var unityServices = new UnityServicesManager();
        await unityServices.Initialize();
        _unityServicesManagerComplete++;
    }

    private async Task AttemptToReInitialize()
    {
        _isInitializedInProcess = true;
        if (UnityServicesManager.isInitialize == false)
            await InitializeUnityServices();
        InitializeUnityPurchasing();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _controller = controller;
        _extensions = extensions;

        if (_initializationCompletionSource?.TrySetResult(true) == false)
            Log.InternalError();
        _initializedComplete++;
        _isInitializedInProcess = false;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        _initializationCompletionSource?.TrySetResult(false);
        Log.Error($"Initialize Failed: {error.ToString()}");
        _initializedFailed++;
        _initializedFailedError = error;
        _initializedFailedMessage = "!";
        _isInitializedInProcess = false;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string? message)
    {
        _initializationCompletionSource?.TrySetResult(false);
        Log.Error($"Initialize Failed: {error.ToString()}; Message: {message}");
        _initializedFailed++;
        _initializedFailedError = error;
        _initializedFailedMessage = message;
        _isInitializedInProcess = false;
    }

    public async Task<PurchaseResponseResult> PurchaseProduct(string productId)
    {
        _purchaseCompletionSource = new TaskCompletionSource<PurchaseResponseResult>();
        if (_controller == null && _isInitializedInProcess == false)
            await AttemptToReInitialize();

        if (_controller == null)
        {
            _purchaseCompletionSource.SetResult(new PurchaseResponseResult
            {
                result = PurchaseResult.Error,
                message = $"Nul_C;{GetInitializedStatus()}",
            });

            return await _purchaseCompletionSource.Task;
        }

        try
        {
            var product = Products?.FirstOrDefault(x => x.ProductId == productId);
            if (product == null)
            {
                _purchaseCompletionSource.With(x => Log.InternalError())
                                         .SetResult(new PurchaseResponseResult
                                         {
                                             result = PurchaseResult.Error,
                                             message = $"Not Found;{GetInitializedStatus()}",
                                         });

                return await _purchaseCompletionSource.Task;
            }

            _controller.InitiatePurchase(productId);
        }
        catch (Exception e)
        {
            _purchaseCompletionSource.With(x => Log.Error(e.Message)).SetResult(new PurchaseResponseResult
            {
                result = PurchaseResult.Error,
                message = e.Message + $";{GetInitializedStatus()}",
            });
        }

        return await _purchaseCompletionSource.Task;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (_purchaseCompletionSource?.TrySetResult(new PurchaseResponseResult
        {
            result = PurchaseResult.Success,
            message = "ProcessPurchase_Method - Success",
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
                    message = "OPM1:" +
                              $"Reason:{failureReason};" +
                              $"Id={product.definition.id};" +
                              $"{GetInitializedStatus()}",
                }) == false)
                    Log.InternalError();

                break;
            default:
                if (_purchaseCompletionSource?.TrySetResult(new PurchaseResponseResult
                {
                    result = PurchaseResult.Error,
                    message = "OPM2:" +
                              $"Reason:{failureReason}" +
                              $"Id={product.definition.id};" +
                              $"{GetInitializedStatus()}",
                }) == false)
                    Log.InternalError();

                break;
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        var purchaseResult = new PurchaseResponseResult
        {
            message = $"OPM3; " +
                      $"ID={failureDescription.productId};" +
                      $"Reason={failureDescription.reason};" +
                      $"Msg={failureDescription.message};" +
                      $"{GetInitializedStatus()}",

            result = failureDescription.reason switch
            {
                PurchaseFailureReason.UserCancelled => PurchaseResult.Cancel,
                _ => PurchaseResult.Error
            },
        };

        if (_purchaseCompletionSource?.TrySetResult(purchaseResult) == false)
            Log.InternalError();
    }

    private string GetInitializedStatus()
    {
        return $"ST={_initStart};" +
               $"USM={_unityServicesManagerComplete};" +
               $"USME={(UnityServicesManager.isInitialize ? 1 : 0)}|{UnityServicesManager.lastError};" +
               $"UP={_unityPurchasingStart};" +
               $"End={_initializedComplete};" +
               $"F={_initializedFailed};" +
               $"FRs={_initializedFailedError};" +
               $"FMs={_initializedFailedMessage};" +
               $"Crl={(_controller != null ? 1 : 0)};" +
               $"Ext={(_extensions != null ? 1 : 0)};" +
               $"IsGP={IsInstallFromGooglePlay()};" +
               $"CT={Products?.Count ?? -1};";
    }

    private static int IsInstallFromGooglePlay()
    {
        try
        {
            using AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager"))
            {
                string packageName = currentActivity.Call<string>("getPackageName");
                var googlePlayPackage = packageManager.Call<string>("getInstallerPackageName", packageName);

                return googlePlayPackage == "com.android.vending" ? 1 : 0;
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }
}
}