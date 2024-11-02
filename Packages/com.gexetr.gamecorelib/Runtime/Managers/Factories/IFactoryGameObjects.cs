using System.Collections.Generic;
using UnityEngine;

namespace Game.Factories
{
public interface IFactoryGameObjects
{
    /// <summary>
    ///  <para>Creates a new empty GameObject</para>
    /// </summary>
    /// <returns>
    ///  <para>Created GameObject</para>
    /// </returns>
    GameObject InstantiateEmpty();

    GameObject InstantiateEmpty(Transform parent);

    GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent);
    
    GameObject InstantiateEmpty(params System.Type[] components);
    
    GameObject InstantiateEmpty(Transform parent, params System.Type[] components);

    GameObject InstantiateEmpty(Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components);
    
    GameObject InstantiateEmpty(string name);

    GameObject InstantiateEmpty(string name, Transform parent);

    GameObject InstantiateEmpty(string name, Vector3 position, Quaternion rotation, Transform parent);
    
    GameObject InstantiateEmpty(string name, params System.Type[] components);
    
    GameObject InstantiateEmpty(string name, Transform parent, params System.Type[] components);

    GameObject InstantiateEmpty(string name, Vector3 position, Quaternion rotation, Transform parent, params System.Type[] components);

    /// <summary>
    ///  <para>Creates a new object from a prefab</para>
    /// </summary>
    /// <param name="prefab">Unity prefab</param>
    /// <returns>
    ///  <para>Created GameObject</para>
    /// </returns>
    GameObject InstantiatePrefab(Object prefab);

    GameObject InstantiatePrefab(Object prefab, Transform parent);

    GameObject InstantiatePrefab(Object prefab, Vector3 position, Quaternion rotation, Transform parent);

    /// <summary>
    ///  <para>Creates a new object from a prefab that already has a component T</para>
    /// </summary>
    /// <param name="prefab">Unity prefab</param>
    /// <typeparam name="T">Existing component on the prefab</typeparam>
    /// <returns>
    ///  <para>Reference to the component</para>
    /// </returns>
    T Instantiate<T>(T prefab) where T : Object;

    T Instantiate<T>(T prefab, IEnumerable<object> extraArgs) where T : Object;

    T Instantiate<T>(T prefab, Transform parent) where T : Object;
    T Instantiate<T>(T prefab, Transform parent, IEnumerable<object> extraArgs) where T : Object;

    T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object;

    T Instantiate<T>(T prefab,
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
    T InstantiateAndAddNewComponent<T>() where T : Component;
    T InstantiateAndAddNewComponent<T>(Transform parent) where T : Component;
    T InstantiateAndAddNewComponent<T>(Transform parent, IEnumerable<object> extraArgs) where T : Component;
    T InstantiateAndAddNewComponent<T>(IEnumerable<object> extraArgs) where T : Component;
    T InstantiateAndAddNewComponent<T>(string name) where T : Component;
    T InstantiateAndAddNewComponent<T>(string name, Transform parent) where T : Component;
    T InstantiateAndAddNewComponent<T>(string name, Transform parent, IEnumerable<object> extraArgs) where T : Component;
    T InstantiateAndAddNewComponent<T>(string name, IEnumerable<object> extraArgs) where T : Component;

    /// <summary>
    ///  <para>Add a new component to an object prefab</para>
    /// </summary>
    /// <param name="gameObject">Object to add component to</param>
    /// <typeparam name="T">Component type</typeparam>
    /// <returns>
    ///  <para>Reference to the component</para>
    /// </returns>
    T AddComponent<T>(GameObject gameObject) where T : Component;

    T AddComponent<T>(GameObject gameObject, IEnumerable<object> extraArgs) where T : Component;
}
}