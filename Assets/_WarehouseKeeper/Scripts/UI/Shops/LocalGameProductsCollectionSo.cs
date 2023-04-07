using UnityEngine;
using WarehouseKeeper.EditorScripts;

namespace WarehouseKeeper.Directors.UI.Shops
{
[CreateAssetMenu(fileName = "LocalProducts", menuName = EditorGameData.EditorName + "/Local products", order = 2)]
internal class LocalGameProductsCollectionSo : ScriptableObject
{
    [field: SerializeField] public LocalGameProduct[] LocalGameProducts { get; private set; }
}
}