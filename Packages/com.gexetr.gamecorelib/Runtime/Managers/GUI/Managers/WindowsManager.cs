using System;
using System.Collections.Generic;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI.Windows.Managers
{
internal class WindowsManager : IDisposable, IWindowsManager
{
    protected readonly WindowBuilder windowBuilder;

    public WindowsManager(IWindowFactory windowFactory, Transform rootUi)
    {
        if (windowFactory.TryCreateWindowsRoot(rootUi, out var root) == false)
            Log.Warning($"In {GetType().Name} empty root");

        windowBuilder = new WindowBuilder(windowFactory, root);
    }

    public void Dispose()
    {
        windowBuilder.Dispose();
    }

    #region Container

    public bool TryGetActiveWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator
    {
        var mediators = new List<TMediator>();
        foreach (WindowProperties window in windowBuilder)
        {
            if (window.mediator is TMediator == false)
                continue;
            mediators.Add((TMediator) window.mediator);
        }

        mediator = mediators.ToArray();

        return mediator.Length > 0;
    }

    public bool TryGetActiveWindow<TMediator>(out TMediator mediator) where TMediator : class, IMediator
    {
        mediator = null;
        foreach (WindowProperties window in windowBuilder)
        {
            if (window.mediator is TMediator == false)
                continue;
            mediator = (TMediator) window.mediator;

            break;
        }

        return mediator != null;
    }

    #endregion

    #region Static transition

    public TMediator OpenWindowOnTop<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        windowBuilder.HideWindow(windowBuilder.Count - 1, false);

        return windowBuilder.OpenWindowSilently(initWindow).mediator as TMediator;
    }

    public TMediator OpenWindowOver<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        windowBuilder.HideWindow(windowBuilder.Count - 1, true);

        return windowBuilder.OpenWindowSilently(initWindow).mediator as TMediator;
    }

    public bool CloseWindow<TMediator>() where TMediator : class, IMediator
    {
        for (var i = 0; i < windowBuilder.Count; i++)
        {
            if (windowBuilder[i].mediator.GetType() != typeof(TMediator))
                continue;

            windowBuilder.CloseWindow(i);

            return true;
        }

        return false;
    }

    public bool CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator
    {
        if (mediator == null)
            return false;

        for (var i = 0; i < windowBuilder.Count; i++)
        {
            if (windowBuilder[i].mediator != mediator)
                continue;

            windowBuilder.CloseWindow(i);

            return true;
        }

        return false;
    }

    public void CloseWindows()
    {
        var countWindows = windowBuilder.Count;
        for (var i = countWindows - 1; i >= 0; i--)
        {
            windowBuilder.CloseWindow(i);
        }
    }

    #endregion
}
}