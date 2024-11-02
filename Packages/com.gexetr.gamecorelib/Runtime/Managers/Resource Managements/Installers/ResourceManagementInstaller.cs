using Game.AssetContent.Managers;

namespace Game.AssetContent.Installers
{
public sealed partial class ResourceManagementInstaller
{
    public static IResourceManagement Addressable() => new AddressablesManager();
    public static IResourceManagement Resources() => new ResourceManagement();
}
}