using UnityEngine;
using WarehouseKeeper.DependencyInjection.Installers;
using Zenject;

namespace WarehouseKeeper.DependencyInjection
{
public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    [SerializeField, Min(0)] private int poolCapacity;
    
    public override void InstallBindings()
    {
        ObjectPoolInstaller.Install(Container, poolCapacity);
    }
}
}