using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WarehouseKeeper.AssetContent;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Factories;
using WarehouseKeeper.Gui.Windows.Mediators;
using Zenject;

namespace WarehouseKeeper.Gui.Windows.WindowsFactories
{
internal class WindowsFactory : IWindowFactory
{
    private readonly DiContainer _container;
    private readonly IAddressablesManager _addressablesManager;
    private readonly IFactoryGameObjects _factory;

    private readonly Dictionary<Type, Type> _windowMediatorMap = new Dictionary<Type, Type>();
    
    public WindowsFactory(DiContainer container, IAddressablesManager addressablesManager, IFactoryGameObjects factory)
    {
        _addressablesManager = addressablesManager;
        _factory = factory;
        _container = container;

        MapMediatorTypes();
    }

    private void MapMediatorTypes()
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
                if(windowType != null) break;
            }

            if (windowType != null)
                _windowMediatorMap.Add(mediator, windowType);
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
     
    public bool TryCreateWindow<TMediator>(Transform root, out TMediator mediator, out Component window) where TMediator : class, IMediator
    {
        mediator = null;
        window = null;
        var mediatorType = typeof(TMediator);
        
        if (_windowMediatorMap.TryGetValue(mediatorType, out var windowType) == false)
        {
            Log.WriteError($"For {mediatorType} Window type not found");

            return false;
        }
        
        var prefabKey = $"UI/{mediatorType.Name.Replace("MediatorUI", "")}";
        var prefab = _addressablesManager.LoadAsset<GameObject>(prefabKey);
        if (prefab == null)
            return false;

        if (prefab.GetComponent(windowType) == null)
        {
            Log.WriteError($"Can't find \"{windowType}\" component in {prefab.gameObject}.");
            return false;
        }
        
        window = _factory.InstantiatePrefab(prefab, root).GetComponent(windowType);
        mediator = _container.Instantiate<TMediator>(new[] {window});

        return mediator != null;
    }
}
}