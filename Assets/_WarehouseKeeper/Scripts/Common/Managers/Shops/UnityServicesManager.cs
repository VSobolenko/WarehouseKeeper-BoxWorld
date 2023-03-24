using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Shops
{
public class UnityServicesManager
{
    private const string EnvironmentKey = "production";

    public Task Initialize()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(EnvironmentKey);
    
            return UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
            Log.WriteError(exception.Message);
        }
        return Task.CompletedTask;
    }
}
}