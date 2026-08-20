using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.UnityServices;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing
{
internal abstract class PurchasingDirector<TIn, TOut> : IPurchasingDirector
    where TOut : StorePurchaseRequestResult<TIn>, new()
{
    private readonly IAPConfigurationCollection _iapCollection;
    private readonly IUnityServicesInitializer _unityServices;

    protected readonly IUnityIAPProvider<TIn, TOut> iapProvider;
    protected readonly HashSet<string> pendingProductsId = new(2);

    public bool HasActivePurchaseTransaction => iapProvider != null && iapProvider.HasActivePurchaseTransaction;
    public event Action<string> OnConfirmedProductBuy;

    protected PurchasingDirector(IUnityIAPProvider<TIn, TOut> iapProvider,
                                 IAPConfigurationCollection iapCollection,
                                 IUnityServicesInitializer unityServices)
    {
        _iapCollection = iapCollection;
        _unityServices = unityServices;

        this.iapProvider = iapProvider;
        if (this.iapProvider != null)
            this.iapProvider.OnProductBuy += item => TryConfirmUnknownPendingPurchases(item);
    }

    public Task InitializeAsync(CancellationToken token)
    {
        if (iapProvider == null)
            return Task.CompletedTask;

        return iapProvider.InitializeAsync(GetSafeProducts(), token);
    }

    public async Task<PurchaseRequestResult> PurchaseProduct(string productId, CancellationToken token)
    {
        pendingProductsId.Add(productId);
        try
        {
            var storeConfirmed = await RequestPurchaseToStore(productId, token);

            if (token.IsCancellationRequested)
                return CreateCanceledResult();

            if (storeConfirmed.result != PurchaseResult.Success)
                return storeConfirmed;

            var serverConfirmed = await FinishPurchaseWithConfirmOnServer(storeConfirmed.item, token);

            if (token.IsCancellationRequested)
                return CreateCanceledResult();

            if (serverConfirmed.result != PurchaseResult.Success)
                return serverConfirmed;

            return serverConfirmed;
        }
        finally
        {
            pendingProductsId.Remove(productId);
        }
    }

    protected abstract Task TryConfirmUnknownPendingPurchases(TIn item);

    protected abstract Task<TOut> FinishPurchaseWithConfirmOnServer(TIn item, CancellationToken token);

    protected async Task<TOut> ConfirmPurchaseOnServer(string transactionId, string receipt, CancellationToken token)
    {
        _ = transactionId;
        _ = receipt;

        // var result = await _server.PostAsync("/purchase/google/check", json?.ToString(), token, headers);
        var result = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(new MessageContainer<string>
            {
                message = "Fake Server",
            })),
        };

        if (token.IsCancellationRequested)
            return CreateCanceledResult();

        if (result.Content == null)
        {
            return await Task.FromResult(new TOut()
            {
                result = PurchaseResult.Error,
                message = "Request Fail",
            });
        }

        var responseJson = await result.Content.ReadAsStringAsync();
        var messageContainer = JsonConvert.DeserializeObject<MessageContainer<string>>(responseJson);

        return await Task.FromResult(new TOut()
        {
            result = result.StatusCode == HttpStatusCode.OK ? PurchaseResult.Success : PurchaseResult.Error,
            message = messageContainer?.message,
        });
    }

    private async Task<TOut> RequestPurchaseToStore(string productId, CancellationToken token)
    {
        try
        {
            if (iapProvider == null)
                return await FinishPurchaseWithException(new InvalidOperationException("IAP provider is not configured."));

            if (iapProvider.IsInitialized == false)
                await AttemptToReInitialize(token);

            if (token.IsCancellationRequested) return CreateCanceledResult();

            if (iapProvider.IsInitialized == false)
                return await FinishMissingInitializePurchase();

            if (token.IsCancellationRequested) return CreateCanceledResult();

            if (_iapCollection == null || _iapCollection.TryGetIAPConfiguration(productId, out _) == false)
                return await FinishUnknownProductPurchase(productId);

            if (token.IsCancellationRequested) return CreateCanceledResult();

            var task = iapProvider.RequestToPurchase(productId);

            return await task;
        }
        catch (Exception e)
        {
            var catchResult = await FinishPurchaseWithException(e);

            return token.IsCancellationRequested ? default : catchResult;
        }
    }

    private static Task<TOut> FinishPurchaseWithException(Exception e) =>
        Task.FromResult(new TOut
        {
            result = PurchaseResult.Error,
            message = e.Message,
        });

    private static Task<TOut> FinishMissingInitializePurchase() =>
        Task.FromResult(new TOut
        {
            result = PurchaseResult.Error,
            message = "IAP Not Initialize",
        });

    private static Task<TOut> FinishUnknownProductPurchase(string productId) =>
        Task.FromResult(new TOut
        {
            result = PurchaseResult.Error,
            message = $"Product \"{productId}\" Not Found",
        });

    private static TOut CreateCanceledResult() => new TOut
    {
        result = PurchaseResult.Cancel,
        message = "Canceled",
    };

    private async Task AttemptToReInitialize(CancellationToken token)
    {
        if (_unityServices != null)
            await _unityServices.InitializeAsync(token);

        if (iapProvider != null)
            await iapProvider.InitializeAsync(GetSafeProducts(), token);
    }

    private IAPConfigurationData[] GetSafeProducts()
    {
        if (_iapCollection == null)
            return Array.Empty<IAPConfigurationData>();

        var products = _iapCollection.products;

        if (products == null || products.Length == 0)
            return Array.Empty<IAPConfigurationData>();

        return products.Where(product => product != null).ToArray();
    }

    protected void InvokeBaseEvent(string id) => OnConfirmedProductBuy?.Invoke(id);

    [Serializable]
    private class MessageContainer<T>
    {
        [JsonProperty("message")] public string message;
        [JsonProperty("data")] public T data;
        [JsonProperty("trace")] public string trace;

        public override string ToString()
        {
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            return string.Join("; ", fields.Select(f => $"{f.Name}={f.GetValue(this)}"));
        }
    }
}
}