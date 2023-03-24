using UnityEngine;

namespace WarehouseKeeper.Pools
{
internal abstract class BasePooledObject : MonoBehaviour, IPoolable
{
    [SerializeField] private string key;

    public string Key => key;

    public virtual bool IsUiElement => false;
    
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

    public void Release() => Pool?.Release(this);

#if UNITY_EDITOR
    
    [ContextMenu("Force start auto validate")]
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(key))
            key = GetType() + "." + name;
    }
    
#endif
}
}