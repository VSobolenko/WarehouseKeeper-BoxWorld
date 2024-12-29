using UnityEngine;
using WarehouseKeeper.EditorScripts;

namespace Game.Shops
{
[CreateAssetMenu(fileName = "ProductsConfig", menuName = EditorGameData.EditorName + "/Products config", order = 2)]
internal class ProductsSettingsCollections : ScriptableObject
{
    [field: SerializeField] public GameProduct[] products;
}
}