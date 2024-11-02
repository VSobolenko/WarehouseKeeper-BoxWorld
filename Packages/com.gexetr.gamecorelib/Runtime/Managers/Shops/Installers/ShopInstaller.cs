using UnityEngine;

namespace Game.Shops.Installers
{
public class ShopInstaller
{
    private const string ResourcesSettingsPath = "Shop/ProductsConfig";
    private static readonly ProductsSettingsCollections Settings;
    
    static ShopInstaller()
    {
        Settings = LoadProductsFromResources();
    }

    public static IShopManager IAP() => new IAPShopManager(Settings.products);
    
    private static ProductsSettingsCollections LoadProductsFromResources()
    {
        var so = Resources.Load<ProductsSettingsCollections>(ResourcesSettingsPath);
        if (so == null)
        {
            Log.Error($"Can't load localization so settings. Path to so: {ResourcesSettingsPath}");

            return default;
        }

        return so;
    }
}
}