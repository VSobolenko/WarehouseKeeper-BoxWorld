using Game.Installers.Factories;
using Game.Installers.GUI;
using Game.Installers.Pools;
using UnityEngine;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.AppearanceWindows.Components.AppearanceItems;
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
using WarehouseKeeper.UI.Windows.LevelSelections;
using WarehouseKeeper.UI.Windows.ShopWindows;
using Zenject;

namespace WarehouseKeeper.DependencyInjection
{
public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    [SerializeField, Min(0)] private int poolCapacity;
    [SerializeField] private Transform uiRoot;
    [SerializeField] private GameCameraController _gameCamera;
    
    public override void InstallBindings()
    {
        InstallBaseManagers();
        InstallDirectors();
        InstallGameComponents();
        InstallFactories();
        InstallMics();
    }

    private void InstallBaseManagers()
    {
        ObjectPoolInstaller.Install(Container, poolCapacity);
        SceneGuiInstaller.Install(Container, uiRoot);
        FactoryInstaller.Install(Container);

        //Container.Bind<IFactoryGameObjects>().To<GameObjectsFactory>().AsSingle();
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
        Container.BindInterfacesAndSelfTo<GameStateMachine>().AsCached();
        Container.BindInterfacesAndSelfTo<GameStateMachineTree>().AsTransient();
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