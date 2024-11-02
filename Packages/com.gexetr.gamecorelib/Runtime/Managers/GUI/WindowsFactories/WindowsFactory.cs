using System;
using System.Collections.Generic;
using System.Linq;
using Game.AssetContent;
using Game.Factories;
using UnityEngine;

namespace Game.GUI.Windows.Factories
{
internal class WindowsFactory : IWindowFactory
{
    private readonly IMediatorInstantiator _container;
    private readonly IResourceManagement _resourceManagement;
    private readonly IFactoryGameObjects _factory;

    private static readonly Dictionary<Type, Type> WindowMediatorMap = new(5);

    public WindowsFactory(IMediatorInstantiator container, IResourceManagement resourceManagement, IFactoryGameObjects factory)
    {
        _resourceManagement = resourceManagement;
        _factory = factory;
        _container = container;

        MapMediatorTypes();
    }

    private static void MapMediatorTypes()
    {
        var mediators = AppDomain.CurrentDomain.GetAssemblies()
                                 .SelectMany(s => s.GetTypes())
                                 .Where(p => typeof(IMediator).IsAssignableFrom(p));

        foreach (var mediator in mediators)
        {
            Type windowType = null;
            foreach (var constructor in mediator.GetConstructors())
            {
                windowType = constructor.GetParameters()
                                        .FirstOrDefault(p => p.ParameterType.IsSubclassOf(typeof(WindowUI)))
                                        ?.ParameterType;

                if (windowType != null) break;
            }

            if (windowType != null)
                WindowMediatorMap.Add(mediator, windowType);
        }
    }

    public bool TryCreateWindowsRoot(Transform root, out Transform uiRoot)
    {
        uiRoot = null;

        var rootPrefab = Resources.Load("UI/UIRoot") as GameObject;

        if (rootPrefab == null)
            return false;

        uiRoot = _factory.InstantiatePrefab(rootPrefab, root).transform;

        return uiRoot != null;
    }

    public bool TryCreateWindow<TMediator>(Transform root, out TMediator mediator, out WindowUI window)
        where TMediator : class, IMediator
    {
        mediator = null;
        window = null;
        var mediatorType = typeof(TMediator);

        if (WindowMediatorMap.TryGetValue(mediatorType, out var windowType) == false)
        {
            Log.Error($"For {mediatorType} Window type not found");

            return false;
        }

        var prefabKey = $"UI/{mediatorType.Name.Replace("MediatorUI", "")}";
        var prefab = _resourceManagement.LoadAsset<GameObject>(prefabKey);

        if (prefab == null)
            return false;

        if (prefab.GetComponent(windowType) == null)
        {
            Log.Error($"Can't find \"{windowType}\" component in {prefab.gameObject}.");

            return false;
        }

        window = _factory.InstantiatePrefab(prefab, root).GetComponent<WindowUI>();
        mediator = _container.Instantiate<TMediator>(window);

        return mediator != null;
    }
}
}