using System.Collections.Generic;
using UnityEngine;

namespace Game.Factories.Managers
{
internal class StandardObjectsFactory : IFactoryGameObjects
{
    private const string NewGameObjectName = "Empty GameObject";

    //Create empty GameObject
    public GameObject InstantiateEmpty() => 
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, null);

    public GameObject InstantiateEmpty(Transform parent) =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, parent);

    public GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent) =>
        InternalInstantiateBase(NewGameObjectName, position, rotation, parent);

    public GameObject InstantiateEmpty(params System.Type[] components) =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, null, components);

    public GameObject InstantiateEmpty(Transform parent, params System.Type[] components) =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, parent, components);

    public GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components) =>
        InternalInstantiateBase(NewGameObjectName, position, rotation, parent, components);

    public GameObject InstantiateEmpty(string name) => 
        InternalInstantiateBase(name, Vector3.zero, Quaternion.identity, null);

    public GameObject InstantiateEmpty(string name, Transform parent) =>
        InternalInstantiateBase(name, Vector3.zero, Quaternion.identity, parent);

    public GameObject InstantiateEmpty(string name, Vector3 position, Quaternion rotation, Transform parent) =>
        InternalInstantiateBase(name, position, rotation, parent);

    public GameObject InstantiateEmpty(string name, params System.Type[] components) =>
        InternalInstantiateBase(name, Vector3.zero, Quaternion.identity, null, components);

    public GameObject InstantiateEmpty(string name, Transform parent, params System.Type[] components) =>
        InternalInstantiateBase(name, Vector3.zero, Quaternion.identity, parent, components);

    public GameObject InstantiateEmpty(string name, Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components) =>
        InternalInstantiateBase(name, position, rotation, parent, components);

    // Creates a new object from a prefab
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
    //ToDo: Use GameObject.Instantiate<T>();
    public T InstantiateAndAddNewComponent<T>() where T : Component =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, null, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(Transform parent) where T : Component =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, parent, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(Transform parent, IEnumerable<object> extraArgs) where T : Component =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, parent, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(IEnumerable<object> extraArgs) where T : Component =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, null, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(string name) where T : Component =>
        InternalInstantiateBase(name, Vector3.zero, Quaternion.identity, null, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(string name, Transform parent) where T : Component =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, parent, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(string name, Transform parent, IEnumerable<object> extraArgs)
        where T : Component =>
        InternalInstantiateBase(NewGameObjectName, Vector3.zero, Quaternion.identity, parent, typeof(T))
            .GetComponent<T>();

    public T InstantiateAndAddNewComponent<T>(string name, IEnumerable<object> extraArgs) where T : Component =>
        InternalInstantiateBase(name, Vector3.zero, Quaternion.identity, null, typeof(T))
            .GetComponent<T>();

    // Add a new component to an object prefab
    public T AddComponent<T>(GameObject gameObject) where T : Component => gameObject.AddComponent<T>();

    public T AddComponent<T>(GameObject gameObject, IEnumerable<object> extraArgs) where T : Component => gameObject.AddComponent<T>();
    
    //ToDo: remove params to single parameter of input Type
    private static GameObject InternalInstantiateBase(string name, Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components)
    {
        GameObject gameObject;
       
        if (components != null && components.Length > 0)
            gameObject = new GameObject(name, components);
        else
            gameObject = new GameObject(name);

        gameObject.transform.SetParent(parent);
        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;

        return gameObject;
    }
    
}
}