#if MONETIZATION_PURCHASING_ENABLE_LEGACY_IAP_NAMESPACE
using System.Threading;
using System.Threading.Tasks;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP
{
[System.Obsolete("Use IAP.UnityServices.UnityServicesManager.")]
internal sealed class UnityServicesManager : IUnityServicesInitializer
{
    private readonly UnityServices.UnityServicesManager _inner = new();

    internal static string LastError => UnityServices.UnityServicesManager.lastError;
    internal static bool IsInitialize => UnityServices.UnityServicesManager.isInitialize;

    public Task InitializeAsync(CancellationToken token) => _inner.InitializeAsync(token);
}
}
#endif
