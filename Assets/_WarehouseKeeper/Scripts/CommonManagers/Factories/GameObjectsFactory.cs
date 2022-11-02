using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WarehouseKeeper.Factories.Managers
{
internal class GameObjectsFactory : IFactoryGameObjects
{
    private readonly DiContainer _container;

    public GameObjectsFactory(DiContainer container)
    {
        _container = container;
    }

    //Create empty GameObject
    public GameObject InstantiateEmpty() => 
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, null);

    public GameObject InstantiateEmpty(Transform parent) =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, parent);

    public GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent) =>
        InternalInstantiateBase(position, rotation, parent);

    public GameObject InstantiateEmpty(params System.Type[] components) =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, null, components);

    public GameObject InstantiateEmpty(Transform parent, params System.Type[] components) =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, parent, components);

    public GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components) =>
        InternalInstantiateBase(position, rotation, parent, components);

    // Creates a new object from a prefab
    public GameObject InstantiatePrefab(Object prefab) => 
        _container.InstantiatePrefab(prefab);

    public GameObject InstantiatePrefab(Object prefab, Transform parent) => 
        _container.InstantiatePrefab(prefab, parent);

    public GameObject InstantiatePrefab(Object prefab, Vector3 position, Quaternion rotation, Transform parent) =>
        _container.InstantiatePrefab(prefab, position, rotation, parent);

    // Creates a new object from a prefab that already has a component T
    public T Instantiate<T>(T prefab) where T : Object => 
        _container.InstantiatePrefabForComponent<T>(prefab);

    public T Instantiate<T>(T prefab, IEnumerable<object> extraArgs) where T : Object =>
        _container.InstantiatePrefabForComponent<T>(prefab, extraArgs);

    public T Instantiate<T>(T prefab, Transform parent) where T : Object =>
        _container.InstantiatePrefabForComponent<T>(prefab, parent);

    public T Instantiate<T>(T prefab, Transform parent, IEnumerable<object> extraArgs) where T : Object =>
        _container.InstantiatePrefabForComponent<T>(prefab, parent, extraArgs);

    public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object =>
        _container.InstantiatePrefabForComponent<T>(prefab, position, rotation, parent);

    public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent,
                            IEnumerable<object> extraArgs) where T : Object =>
        _container.InstantiatePrefabForComponent<T>(prefab, position, rotation, parent, extraArgs);

    // Creates a new object and adds a new component to it
    public T InstantiateAndAddNewComponent<T>() where T : Component =>
        _container.InstantiateComponentOnNewGameObject<T>();

    public T InstantiateAndAddNewComponent<T>(string gameObjectName) where T : Component =>
        _container.InstantiateComponentOnNewGameObject<T>(gameObjectName);

    public T InstantiateAndAddNewComponent<T>(IEnumerable<object> extraArgs) where T : Component =>
        _container.InstantiateComponentOnNewGameObject<T>(extraArgs);

    public T InstantiateAndAddNewComponent<T>(string gameObjectName, IEnumerable<object> extraArgs)
        where T : Component => _container.InstantiateComponentOnNewGameObject<T>(gameObjectName, extraArgs);

    // Add a new component to an object prefab
    public T AddComponent<T>(GameObject gameObject) where T : Component =>
        _container.InstantiateComponent<T>(gameObject);

    public T AddComponent<T>(GameObject gameObject, IEnumerable<object> extraArgs) where T : Component =>
        _container.InstantiateComponent<T>(gameObject, extraArgs);

    private static GameObject InternalInstantiateBase(Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components)
    {
        GameObject gameObject;
        const string newGameObjectName = "Empty GameObject";
       
        if (components != null && components.Length > 0)
            gameObject = new GameObject(newGameObjectName, components);
        else
            gameObject = new GameObject(newGameObjectName);

        gameObject.transform.SetParent(parent);
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;

        return gameObject;
    }
}
}