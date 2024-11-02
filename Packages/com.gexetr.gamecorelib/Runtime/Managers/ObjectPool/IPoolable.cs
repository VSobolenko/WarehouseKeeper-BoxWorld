using UnityEngine;

namespace Game.Pools
{
public interface IPoolable
{
    string Key { get; }
    
    bool IsUiElement { get; }
    
    IObjectPoolRecyclable Pool { set; }
    
    void SetParent(Transform parent);
    
    void SetPositionAndRotation(Vector3 position, Quaternion rotation);
    
    void SetActive(bool status);

    void OnUse();
    
    void OnRelease();
}
}