using System.Collections.Generic;
using UnityEngine;

namespace WarehouseKeeper.Factories.Managers
{
public class StandardObjectsFactory : IFactoryGameObjects
{
    private const string NewGameObjectName = "Empty GameObject";

    //Create empty GameObject
    public GameObject InstantiateEmpty() => 
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, null, NewGameObjectName);

    public GameObject InstantiateEmpty(Transform parent) =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, parent, NewGameObjectName);

    public GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent) =>
        InternalInstantiateBase(position, rotation, parent, NewGameObjectName);

    public GameObject InstantiateEmpty(params System.Type[] components) =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, null, NewGameObjectName, components);

    public GameObject InstantiateEmpty(Transform parent, params System.Type[] components) =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, parent, NewGameObjectName, components);

    public GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components) =>
        InternalInstantiateBase(position, rotation, parent, NewGameObjectName, components);

    public GameObject InstantiatePrefab(Object prefab) => Object.Instantiate(prefab) as GameObject;

    public GameObject InstantiatePrefab(Object prefab, Transform parent) => 
        Object.Instantiate(prefab, parent) as GameObject;

    public GameObject InstantiatePrefab(Object prefab, Vector3 position, Quaternion rotation, Transform parent) =>
        Object.Instantiate(prefab, position, rotation, parent) as GameObject;

    // Creates a new object from a prefab that already has a component T
    public T Instantiate<T>(T prefab) where T : Object => Object.Instantiate(prefab);

    public T Instantiate<T>(T prefab, IEnumerable<object> extraArgs) where T : Object => 
        Object.Instantiate(prefab);

    public T Instantiate<T>(T prefab, Transform parent) where T : Object => 
        Object.Instantiate(prefab, parent);

    public T Instantiate<T>(T prefab, Transform parent, IEnumerable<object> extraArgs) where T : Object =>
        Object.Instantiate(prefab, parent);

    public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object =>
        Object.Instantiate(prefab, position, rotation, parent);

    public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent,
                            IEnumerable<object> extraArgs) where T : Object =>
        Object.Instantiate(prefab, position, rotation, parent);
    
    // Creates a new object and adds a new component to it
    public T InstantiateAndAddNewComponent<T>() where T : Component =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, null, NewGameObjectName, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(string gameObjectName) where T : Component =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, null, gameObjectName, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(IEnumerable<object> extraArgs) where T : Component =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, null, NewGameObjectName, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(string gameObjectName, IEnumerable<object> extraArgs) where T : Component =>
        InternalInstantiateBase(Vector3.zero, Quaternion.identity, null, gameObjectName, typeof(T))
            .GetComponent<T>();

    // Add a new component to an object prefab
    public T AddComponent<T>(GameObject gameObject) where T : Component => gameObject.AddComponent<T>();

    public T AddComponent<T>(GameObject gameObject, IEnumerable<object> extraArgs) where T : Component => gameObject.AddComponent<T>();
    
    private static GameObject InternalInstantiateBase(Vector3 position, Quaternion rotation, Transform parent, string gameObjectName, params System.Type[] components)
    {
        GameObject gameObject;
       
        if (components != null && components.Length > 0)
            gameObject = new GameObject(gameObjectName, components);
        else
            gameObject = new GameObject(gameObjectName);

        gameObject.transform.SetParent(parent);
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;

        return gameObject;
    }
    
}
}