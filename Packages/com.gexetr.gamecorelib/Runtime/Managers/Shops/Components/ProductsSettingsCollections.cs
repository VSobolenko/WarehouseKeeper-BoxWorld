using Game.InternalData;
using UnityEngine;

namespace Game.Shops
{
[CreateAssetMenu(fileName = "ProductsConfig", menuName = GameData.EditorName + "/Products config", order = 2)]
internal class ProductsSettingsCollections : ScriptableObject
{
    [field: SerializeField] public GameProduct[] products;
}
}