using UnityEngine;
using WarehouseKeeper.Gui.Windows.Mediators;

namespace WarehouseKeeper.Gui.Windows.WindowsFactories
{
internal interface IWindowFactory
{
    public bool TryCreateWindowsRoot(Transform root, out Transform uiRoot);
    
    public bool TryCreateWindow<TMediator>(Transform root, out TMediator mediator, out Component window) where TMediator : class, IMediator;
}
}