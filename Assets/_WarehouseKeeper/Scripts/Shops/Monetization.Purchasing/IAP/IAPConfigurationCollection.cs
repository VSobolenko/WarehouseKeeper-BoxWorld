using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP
{
[CreateAssetMenu(fileName = nameof(IAPConfigurationCollection), menuName = "Purchasing/Product Configuration", order = 1)]
internal sealed class IAPConfigurationCollection : ScriptableObject
{
    public IAPConfigurationData[] products;

    private IAPConfigurationData[] SafeProducts => products ?? Array.Empty<IAPConfigurationData>();

    public IAPConfigurationData GetIAPConfiguration(PurchaseItem purchaseItem)
    {
        foreach (var configurationData in SafeProducts)
            if (configurationData.purchaseItem == purchaseItem)
                return configurationData;

        throw new UnknownItemIAPConfigurationData("Unknown item: " + purchaseItem);
    }

    public IAPConfigurationData GetIAPConfiguration(string productId)
    {
        foreach (var configurationData in SafeProducts)
            if (configurationData.id == productId)
                return configurationData;

        throw new UnknownIdIAPConfigurationData("Unknown id: " + productId);
    }

    public bool TryGetIAPConfiguration(string productId, out IAPConfigurationData data)
    {
        data = null;
        foreach (var configurationData in SafeProducts)
        {
            if (configurationData.id != productId)
                continue;

            data = configurationData;

            return true;
        }

        return data == null;
    }
}

[Serializable]
internal sealed class IAPConfigurationData
{
    public string id;
    public ProductType productType;
    public PurchaseItem purchaseItem;
}

internal enum PurchaseItem : byte
{
    None = 0,
}
}