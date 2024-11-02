using System.Collections.Generic;
using Game.Factories;
using UnityEngine;
using Zenject;

namespace GameTests
{
public class DependencyInjectionFactory : IFactoryGameObjects
{
    private readonly DiContainer _container;

    private const string NewGameObjectName = "Empty GameObject";
    
    public DependencyInjectionFactory(DiContainer container)
    {
        _container = container;
    }

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

    public T InstantiateAndAddNewComponent<T>(Transform parent) where T : Component
    {
        var component = _container.InstantiateComponentOnNewGameObject<T>();
        component.transform.SetParent(parent);

        return component;
    }

    public T InstantiateAndAddNewComponent<T>(Transform parent, IEnumerable<object> extraArgs) where T : Component
    {
        var component = _container.InstantiateComponentOnNewGameObject<T>(extraArgs);
        component.transform.SetParent(parent);

        return component;
    }

    public T InstantiateAndAddNewComponent<T>(IEnumerable<object> extraArgs) where T : Component =>
        _container.InstantiateComponentOnNewGameObject<T>(extraArgs);
    
    public T InstantiateAndAddNewComponent<T>(string name) where T : Component =>
        _container.InstantiateComponentOnNewGameObject<T>(name);

    public T InstantiateAndAddNewComponent<T>(string name, Transform parent) where T : Component
    {
        var component = _container.InstantiateComponentOnNewGameObject<T>(name);
        component.transform.SetParent(parent);

        return component;
    }

    public T InstantiateAndAddNewComponent<T>(string name, Transform parent, IEnumerable<object> extraArgs)
        where T : Component
    {
        var component = _container.InstantiateComponentOnNewGameObject<T>(name ,extraArgs);
        component.transform.SetParent(parent);

        return component;
    }
    
    public T InstantiateAndAddNewComponent<T>(string name, IEnumerable<object> extraArgs)
        where T : Component => _container.InstantiateComponentOnNewGameObject<T>(name, extraArgs);

    // Add a new component to an object prefab
    public T AddComponent<T>(GameObject gameObject) where T : Component =>
        _container.InstantiateComponent<T>(gameObject);

    public T AddComponent<T>(GameObject gameObject, IEnumerable<object> extraArgs) where T : Component =>
        _container.InstantiateComponent<T>(gameObject, extraArgs);

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