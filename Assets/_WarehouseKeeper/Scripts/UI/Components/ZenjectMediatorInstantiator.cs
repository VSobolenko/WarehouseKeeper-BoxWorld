using Game.GUI.Windows;
using Game.GUI.Windows.Factories;
using Zenject;

namespace WarehouseKeeper.UI.Windows
{
public class ZenjectMediatorInstantiator : IMediatorInstantiator
{
    private readonly DiContainer _container;

    public ZenjectMediatorInstantiator(DiContainer container)
    {
        _container = container;
    }

    public TMediator Instantiate<TMediator>(WindowUI windowUI) where TMediator : class, IMediator =>
        _container.Instantiate<TMediator>(new[] {windowUI});
}
}