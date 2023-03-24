using System;
using System.Threading.Tasks;
using WarehouseKeeper.Gui.Windows.Mediators;
using WarehouseKeeper.Gui.Windows.Transitions;

namespace WarehouseKeeper.Gui.Windows
{
internal interface IWindowsManagerAsync : IWindowsContainer
{
    Task<TMediator> OpenWindowOnTopAsync<TMediator>(Action<TMediator> initWindow = null) where TMediator : class, IMediator;
    Task<TMediator> OpenWindowOverAsync<TMediator>(Action<TMediator> initWindow = null) where TMediator : class, IMediator;

    Task<TMediator> OpenWindowOnTopAsync<TMediator>(IWindowTransition transition = null, Action<TMediator> initWindow = null) where TMediator : class, IMediator;
    Task<TMediator> OpenWindowOverAsync<TMediator>(IWindowTransition transition = null, Action<TMediator> initWindow = null) where TMediator : class, IMediator;
    
    Task<bool> CloseWindowAsync<TMediator>() where TMediator : class, IMediator;
    
    Task<bool> CloseWindowAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator;
}
}