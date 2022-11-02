using UnityEngine;
using UnityEngine.UI;
using WarehouseKeeper.Factories;

namespace WarehouseKeeper.Pools.Managers
{
internal class BaseObjectPoolManager
{
    protected readonly IFactoryGameObjects factoryGameObjects;
    private readonly Transform _root;
    private readonly Transform _rootUi;

    protected BaseObjectPoolManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot)
    {
        factoryGameObjects = objectFactoryGameObjects;
        
        _root = objectFactoryGameObjects.InstantiateEmpty(poolRoot).transform;

        _rootUi = objectFactoryGameObjects
                  .InstantiateEmpty(poolRoot, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                                    typeof(GraphicRaycaster)).transform;
        
        _rootUi.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        
#if UNITY_EDITOR
        if (poolRoot != null)
            poolRoot.name = $"{Application.productName}.Pool";

        _root.name = $"{Application.productName}.Root";
        _rootUi.name = $"{Application.productName}.RootUI";
#endif
        
    }

    protected Transform GetPoolRoot(IPoolable poolableObject) => poolableObject.IsUiElement ? _rootUi : _root;
}
}