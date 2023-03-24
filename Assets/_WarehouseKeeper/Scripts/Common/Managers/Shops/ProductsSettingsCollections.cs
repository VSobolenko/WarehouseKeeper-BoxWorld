using UnityEngine;

namespace WarehouseKeeper.Shops
{
[CreateAssetMenu(fileName = "ProductsConfig", menuName = "Warehouse Keeper/Products config", order = 2)]
internal class ProductsSettingsCollections : ScriptableObject
{
    [field: SerializeField] public GameProduct[] products;
}
}