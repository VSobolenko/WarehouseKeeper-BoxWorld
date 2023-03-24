using UnityEngine;

namespace WarehouseKeeper.Directors.UI.Shops
{
[CreateAssetMenu(fileName = "LocalProducts", menuName = "Warehouse Keeper/Local products", order = 2)]
internal class LocalGameProductsCollection : ScriptableObject
{
    [field: SerializeField] public LocalGameProduct[] LocalGameProducts { get; private set; }
}
}