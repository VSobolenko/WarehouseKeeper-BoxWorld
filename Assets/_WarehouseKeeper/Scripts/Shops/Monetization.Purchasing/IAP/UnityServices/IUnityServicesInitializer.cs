using System.Threading;
using System.Threading.Tasks;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.UnityServices
{
internal interface IUnityServicesInitializer
{
    Task InitializeAsync(CancellationToken token);
}
}


