using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Shops
{
internal sealed class ResourcesShopCatalog : IShopCatalog
{
    private const string ResourcesSettingsPath = "Shop/ProductsConfig";
    private readonly HashSet<GameProduct> _products;

    public IReadOnlyCollection<GameProduct> Products => _products;

    public ResourcesShopCatalog()
    {
        var settings = Resources.Load<ProductsSettingsCollections>(ResourcesSettingsPath);
        if (settings == null || settings.products == null)
            throw new InvalidOperationException($"Can't load products from Resources/{ResourcesSettingsPath}");

        _products = settings.products
                            .Where(product => product != null && product.Ignored == false)
                            .ToHashSet();
    }
}
}

