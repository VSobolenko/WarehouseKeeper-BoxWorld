using UnityEngine;
using WarehouseKeeper.Extension;
using Zenject;

namespace WarehouseKeeper.Shops
{
public class ShopInstaller : Installer<ShopInstaller>
{
    private const string ResourcesSettingsPath = "Shop/ProductsConfig";
    
    public override void InstallBindings()
    {
        Container.Bind<IShopManager>().To<IAPShopManager>().AsSingle().NonLazy();
        Container.Bind<GameProduct[]>().FromMethod(LoadProductsFromResources).WhenInjectedInto<IShopManager>();
    }

    private GameProduct[] LoadProductsFromResources()
    {
        var so = Resources.Load<ProductsSettingsCollections>(ResourcesSettingsPath);
        if (so == null)
        {
            Log.WriteError($"Can't load localization so settings. Path to so: {ResourcesSettingsPath}");

            return default;
        }

        return so.products;
    }
}
}