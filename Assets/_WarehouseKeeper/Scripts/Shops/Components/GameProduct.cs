using System;
using UnityEngine;

namespace Game.Shops
{
[Serializable]
public class GameProduct
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
public struct ShopProductReward
{
    public RewardType type;
    public int quantity;
}

public enum RewardType : byte
{
    RemoveAds,
    Hint,
    Amber,
}

public enum ProductType
{
    Consumable,
    NonConsumable,
    Subscription
}
}