using System;
using System.Threading.Tasks;
using Game.GUI.Windows.Transitions;

namespace Game.GUI.Windows
{
public interface IWindowsManagerAsync : IWindowsManager
{
    Task<TMediator> OpenWindowOnTopAsync<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator;

    Task<TMediator> OpenWindowOnTopAsync<TMediator>(IWindowTransition transition, Action<TMediator> initWindow = null)
        where TMediator : class, IMediator;

    Task<bool> CloseWindowAsync<TMediator>() where TMediator : class, IMediator;

    Task<bool> CloseWindowAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator;
}
}