using UnityEngine;

namespace Game.Pools
{
public abstract class BasePooledObject : MonoBehaviour, IPoolable
{
    public virtual string Key => string.Empty;

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
}
}