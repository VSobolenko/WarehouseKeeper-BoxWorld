using UnityEngine;
using WarehouseKeeper.Pools.Managers;

namespace WarehouseKeeper.Pools
{
internal abstract class BasePooledObject : MonoBehaviour, IPoolable
{
    [SerializeField] private string key;

    public string Key => key;
    
    public bool IsUiElement { get; set; }
    
    public IObjectPoolRecyclable Pool { get; set; }

    public virtual void SetParent(Transform parent) => transform.SetParent(parent);

    public virtual void SetPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        transform.localPosition = position;
        transform.localRotation = rotation;
    }
    
    public virtual void SetActive(bool value) => gameObject.SetActive(value);

    public virtual void OnUse() { }

    public virtual void OnRelease() { }

    private void Reset()
    {
        key = GetType().ToString();
    }
}
}