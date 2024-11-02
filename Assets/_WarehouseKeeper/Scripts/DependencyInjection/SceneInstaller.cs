using System;
using System.Collections.Generic;
using Game.AssetContent;
using Game.Factories;
using Game.Factories.Installers;
using Game.GUI.Installers;
using Game.GUI.Windows;
using Game.Localizations;
using Game.Pools;
using Game.Pools.Installers;
using UnityEngine;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.AppearanceWindows.Components.AppearanceItems;
using WarehouseKeeper.DependencyInjection.ZenjectDependency;
using WarehouseKeeper.Directors;
using WarehouseKeeper.Directors.Game;
using WarehouseKeeper.Directors.Game.Ads;
using WarehouseKeeper.Directors.Game.Analytics;
using WarehouseKeeper.Directors.Game.Audio;
using WarehouseKeeper.Directors.Game.Game_FSM;
using WarehouseKeeper.Directors.Game.SceneData;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Shops;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows;
using WarehouseKeeper.UI.Windows.LevelSelections;
using WarehouseKeeper.UI.Windows.ShopWindows;
using Zenject;

namespace WarehouseKeeper.DependencyInjection
{
public class SceneInstaller : MonoInstaller
{
    [SerializeField, Min(0)] private int poolCapacity;
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameCameraController _gameCamera;
    
    public override void InstallBindings()
    {
        InstallBaseManagers();
        InstallUI();
        InstallDirectors();
        InstallGameComponents();
        InstallFactories();
        InstallMics();
    }

    private void InstallBaseManagers()
    {
        Container.Bind<IFactoryGameObjects>().To<DependencyInjectionFactory>().AsSingle();
        var factory = Container.Resolve<IFactoryGameObjects>();
        var pool = ObjectPoolInstaller.KeyAutoEditor(factory, null, poolCapacity);
        Container.Bind<IObjectPoolManager>().FromInstance(pool).AsSingle().NonLazy();
        //ObjectPoolInstallerZenject.Install(Container, poolCapacity);
        //SceneGuiInstaller.Install(Container, uiRoot);
    }

    private void InstallUI()
    {
        var mediatorInstaller = new ZenjectMediatorInstantiator(Container);
        var resourceManagement = Container.Resolve<IResourceManagement>();
        var factory = Container.Resolve<IFactoryGameObjects>();
        var transition = GuiInstaller.VerticalTransition();
        var uiManager = GuiInstaller.ManagerAsync(mediatorInstaller, resourceManagement, factory, transition, uiRoot);
        Container.BindInterfacesAndSelfTo<IWindowsManagerAsync>().FromInstance(uiManager);
    }

    private void InstallDirectors()
    {
        Container.BindInterfacesAndSelfTo<LevelBuilder>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelDirector>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelProgressDirector>().AsSingle();

        Container.BindInterfacesAndSelfTo<PlayerResourcesDirector>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<WindowsDirector>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameDirector>().AsSingle();
        Container.BindInterfacesAndSelfTo<ResourcesDirector>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AudioDirector>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ShopDirector>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AnalyticsDirector>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AdsDirector>().AsSingle().NonLazy();

    }

    private void InstallGameComponents()
    {
        Container.BindInterfacesAndSelfTo<GameStateMachineBuilder>().AsTransient();
        Container.BindInterfacesAndSelfTo<LevelHint>().AsTransient();
        Container.Bind<GameCameraController>().FromInstance(_gameCamera).AsSingle();
    }

    private void InstallFactories()
    {
        Container.BindInterfacesAndSelfTo<LevelSelectionItemFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AppearanceItemFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AudioFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ShopItemFactory>().AsSingle().NonLazy();
    }

    private void InstallMics()
    {
        
    }
}
}