using System.Collections.Generic;
using UnityEngine;

namespace WarehouseKeeper.Factories
{
internal interface IFactoryGameObjects
{
    /// <summary>
    ///  <para>Creates a new empty GameObject</para>
    /// </summary>
    /// <returns>
    ///  <para>Created GameObject</para>
    /// </returns>
    public GameObject InstantiateEmpty();

    public GameObject InstantiateEmpty(Transform parent);

    public GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent);
    
    public GameObject InstantiateEmpty(params System.Type[] components);
    
    public GameObject InstantiateEmpty(Transform parent, params System.Type[] components);

    public GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components);
    
    public GameObject InstantiateEmpty(string name);

    public GameObject InstantiateEmpty(string name, Transform parent);

    public GameObject InstantiateEmpty(string name, Vector3 position, Quaternion rotation, Transform parent);
    
    public GameObject InstantiateEmpty(string name, params System.Type[] components);
    
    public GameObject InstantiateEmpty(string name, Transform parent, params System.Type[] components);

    public GameObject InstantiateEmpty(string name, Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components);

    /// <summary>
    ///  <para>Creates a new object from a prefab</para>
    /// </summary>
    /// <param name="prefab">Unity prefab</param>
    /// <returns>
    ///  <para>Created GameObject</para>
    /// </returns>
    public GameObject InstantiatePrefab(Object prefab);

    public GameObject InstantiatePrefab(Object prefab, Transform parent);

    public GameObject InstantiatePrefab(Object prefab, Vector3 position, Quaternion rotation, Transform parent);

    /// <summary>
    ///  <para>Creates a new object from a prefab that already has a component T</para>
    /// </summary>
    /// <param name="prefab">Unity prefab</param>
    /// <typeparam name="T">Existing component on the prefab</typeparam>
    /// <returns>
    ///  <para>Reference to the component</para>
    /// </returns>
    public T Instantiate<T>(T prefab) where T : Object;

    public T Instantiate<T>(T prefab, IEnumerable<object> extraArgs) where T : Object;

    public T Instantiate<T>(T prefab, Transform parent) where T : Object;
    public T Instantiate<T>(T prefab, Transform parent, IEnumerable<object> extraArgs) where T : Object;

    public T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object;

    public T Instantiate<T>(T prefab,
                            Vector3 position,
                            Quaternion rotation,
                            Transform parent,
                            IEnumerable<object> extraArgs) where T : Object;

    //ToDo: Add normal documentation
    /// <summary>
    ///  <para>Creates a new object and adds a new component to it</para>
    /// </summary>
    /// <param name="prefab">Unity prefab</param>
    /// <typeparam name="T">Existing component on the prefab</typeparam>
    /// <returns>
    ///  <para>Reference to the component</para>
    /// </returns>
    public T InstantiateAndAddNewComponent<T>() where T : Component;
    public T InstantiateAndAddNewComponent<T>(Transform parent) where T : Component;
    public T InstantiateAndAddNewComponent<T>(Transform parent, IEnumerable<object> extraArgs) where T : Component;
    public T InstantiateAndAddNewComponent<T>(IEnumerable<object> extraArgs) where T : Component;
    public T InstantiateAndAddNewComponent<T>(string name) where T : Component;
    public T InstantiateAndAddNewComponent<T>(string name, Transform parent) where T : Component;
    public T InstantiateAndAddNewComponent<T>(string name, Transform parent, IEnumerable<object> extraArgs) where T : Component;
    public T InstantiateAndAddNewComponent<T>(string name, IEnumerable<object> extraArgs) where T : Component;

    /// <summary>
    ///  <para>Add a new component to an object prefab</para>
    /// </summary>
    /// <param name="gameObject">Object to add component to</param>
    /// <typeparam name="T">Component type</typeparam>
    /// <returns>
    ///  <para>Reference to the component</para>
    /// </returns>
    public T AddComponent<T>(GameObject gameObject) where T : Component;

    public T AddComponent<T>(GameObject gameObject, IEnumerable<object> extraArgs) where T : Component;
}
}