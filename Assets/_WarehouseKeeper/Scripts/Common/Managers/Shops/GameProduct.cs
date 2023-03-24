using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace WarehouseKeeper.Shops
{
[Serializable]
internal class GameProduct
{
    [field: SerializeField] public string ProductId { get; private set; }
    [field: SerializeField] public bool Ignored { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField] public ProductType Type { get; private set; }
    [field: SerializeField] public string AddressableItemKey { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string LocalizationKeyName { get; private set; }
    [field: SerializeField] public ShopProductReward[] Rewards { get; private set; }
}

[Serializable]
internal struct ShopProductReward
{
    public RewardType type;
    public int quantity;
}

internal enum RewardType : byte
{
    RemoveAds,
    Hint,
    Amber,
}
}