using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.UnityServices
{
internal sealed class UnityServicesManager : IUnityServicesInitializer
{
    internal static string lastError = string.Empty;
    internal static bool isInitialize;

    public async Task InitializeAsync(CancellationToken token)
    {
        try
        {
            isInitialize = false;
            await Unity.Services.Core.UnityServices.InitializeAsync();

            if (token.IsCancellationRequested)
                return;

            isInitialize = true;
            lastError = string.Empty;
        }
        catch (Exception exception)
        {
            lastError = exception.Message;
            Debug.LogError(lastError);
        }
    }
}
}

