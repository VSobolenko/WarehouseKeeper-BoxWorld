using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace Game.Shops
{
internal class UnityServicesManager
{
    internal static string lastError = string.Empty;
    internal static bool isInitialize;
    private const string EnvironmentKey = "production";

    public async Task Initialize()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(EnvironmentKey);
    
            await UnityServices.InitializeAsync(options);
            isInitialize = true;
        }
        catch (Exception exception)
        {
            lastError = exception.Message;
            Log.Error(lastError);
        }
    }
}
}