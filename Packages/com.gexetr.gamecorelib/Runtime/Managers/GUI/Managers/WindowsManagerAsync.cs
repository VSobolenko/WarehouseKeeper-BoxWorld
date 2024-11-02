using System;
using System.Threading.Tasks;
using Game.GUI.Windows.Factories;
using Game.GUI.Windows.Transitions;
using UnityEngine;

namespace Game.GUI.Windows.Managers
{
internal class WindowsManagerAsync : WindowsManager, IWindowsManagerAsync
{
    private readonly IWindowTransition _defaultTransition;

    public WindowsManagerAsync(IWindowFactory windowFactory, Transform rootUi, WindowSettings settings,
                               IWindowTransition defaultTransition) : base(windowFactory, rootUi)
    {
        _defaultTransition = defaultTransition;
    }

    public async Task<TMediator> OpenWindowOnTopAsync<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(_defaultTransition, false, initWindow);

    public async Task<TMediator> OpenWindowOverAsync<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(_defaultTransition, true, initWindow);

    public async Task<TMediator> OpenWindowOnTopAsync<TMediator>(IWindowTransition transition = null,
                                                                 Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(transition, false, initWindow);

    public async Task<TMediator> OpenWindowOverAsync<TMediator>(IWindowTransition transition = null,
                                                                Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(transition, true, initWindow);

    private async Task<TMediator> OpenWindowAsync<TMediator>(IWindowTransition transition, bool deactivateLastWindow,
                                                             Action<TMediator> initWindow)
        where TMediator : class, IMediator
    {
        var countWindows = windowBuilder.Count;
        var closeTask = countWindows > 0
            ? transition.Close(windowBuilder[countWindows - 1])
            : Task.CompletedTask;
        var windowData = windowBuilder.OpenWindowSilently(initWindow);
        var openTask = transition != null
            ? transition.Open(windowData)
            : Task.CompletedTask;

        await Task.WhenAll(closeTask, openTask);

        return windowData.mediator as TMediator;
    }

    public async Task<bool> CloseWindowAsync<TMediator>() where TMediator : class, IMediator
    {
        for (var i = 0; i < windowBuilder.Count; i++)
        {
            if (windowBuilder[i].mediator.GetType() != typeof(TMediator))
                continue;

            var closingWindows = windowBuilder[i];
            var openingWindow = i == windowBuilder.Count - 1 ? default : windowBuilder[^1];
            var result = await CloseWindowAsync(closingWindows, openingWindow, i, _defaultTransition);

            return result;
        }

        return false;
    }

    // DANGER - похожий метод, посмотреть
    public async Task<bool> CloseWindowAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator
    {
        for (var i = 0; i < windowBuilder.Count; i++)
        {
            if (windowBuilder[i].mediator != mediator)
                continue;

            var closingWindows = windowBuilder[i];
            var openingWindow = windowBuilder.Count == 1 ? default : windowBuilder[i - 1];
            var result = await CloseWindowAsync(closingWindows, openingWindow, i, _defaultTransition);

            return result;
        }

        return false;
    }

    private async Task<bool> CloseWindowAsync(WindowProperties closingWindow, WindowProperties openingWindow,
                                              int closingWindowIndex, IWindowTransition transition)
    {
        transition = _defaultTransition;
        var closeTask = closingWindow.mediator != null && transition != null
            ? transition.Close(closingWindow)
            : Task.CompletedTask;
        var openTask = openingWindow.mediator != null && transition != null
            ? transition.Open(openingWindow)
            : Task.CompletedTask;

        await Task.WhenAll(closeTask, openTask);
        openingWindow.mediator?.SetActive(true);
        windowBuilder.CloseWindow(closingWindowIndex);

        return true;
    }
}
}