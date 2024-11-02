using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace Game.Shops
{
internal class UnityServicesManager
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
            Log.Error(exception.Message);
        }
        return Task.CompletedTask;
    }
}
}