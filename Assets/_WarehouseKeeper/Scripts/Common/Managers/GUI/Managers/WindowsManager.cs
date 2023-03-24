using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Gui.Windows.Mediators;
using WarehouseKeeper.Gui.Windows.Transitions;
using WarehouseKeeper.Gui.Windows.WindowsFactories;

namespace WarehouseKeeper.Gui.Windows
{
internal class WindowsManager : IDisposable, IWindowsManager, IWindowsManagerAsync
{
    private readonly IWindowFactory _windowFactory;
    private readonly IWindowTransition _windowTransition;

    private readonly Transform _root;
    private readonly List<WindowData> _windows = new List<WindowData>(8);
    protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    
    public WindowsManager(IWindowFactory windowFactory, IWindowTransition windowTransition, Transform rootUi)
    {
        _windowFactory = windowFactory;
        _windowTransition = windowTransition;

        if (_windowFactory.TryCreateWindowsRoot(rootUi, out _root) == false) 
            Log.WriteWarning($"In {GetType()} empty root");
    }
    
    public void Dispose()
    {
        for (var i = _windows.Count - 1; i >= 0; i--)
        {
            try
            {
                CloseWindow(i);
            }
            catch (Exception e)
            {
                Log.WriteError($"Error inside close: {e.Message}");
            }
        }
        _windows.Clear();
    }

    #region Container

    public bool TryGetActiveWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator
    {
        var mediators = new List<TMediator>();
        foreach (var window in _windows)
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
        foreach (var window in _windows)
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
        HideWindow(_windows.Count - 1, false);
        return OpenWindow(initWindow).mediator as TMediator;    
    }

    public TMediator OpenWindowOver<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        HideWindow(_windows.Count - 1, true);
        return OpenWindow(initWindow).mediator as TMediator;
    }

    public bool CloseWindow<TMediator>() where TMediator : class, IMediator
    {
        for (var i = 0; i < _windows.Count; i++)
        {
            if (_windows[i].mediator.GetType() != typeof(TMediator)) 
                continue;
            
            CloseWindow(i);
            return true;
        }

        return false;
    }
    
    public bool CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator
    {
        if (mediator == null)
            return false;
        
        for (var i = 0; i < _windows.Count; i++)
        {
            if (_windows[i].mediator != mediator) 
                continue;
            
            CloseWindow(i);
            return true;
        }

        return false;
    }

    public void CloseWindows()
    {
        var countWindows = _windows.Count;
        for (var i = countWindows - 1; i >= 0; i--)
        {
            CloseWindow(i);
        }
    }

    #endregion

    #region Async transition

    public async Task<TMediator> OpenWindowOnTopAsync<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(_windowTransition, false, initWindow);

    public async Task<TMediator> OpenWindowOverAsync<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(_windowTransition, true, initWindow);

    public async Task<TMediator> OpenWindowOnTopAsync<TMediator>(IWindowTransition transition = null,
                                                                 Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(transition, false, initWindow);

    public async Task<TMediator> OpenWindowOverAsync<TMediator>(IWindowTransition transition = null,
                                                                Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(transition, true, initWindow);

    private async Task<TMediator> OpenWindowAsync<TMediator>(IWindowTransition transition, bool deactivateLastWindow, Action<TMediator> initWindow)
        where TMediator : class, IMediator
    {
        var closeTask = _windows.Count > 0 && transition != null
            ? transition.Close(_windows[^1].rectTransform, _windows[^1].canvasGroup)
            : Task.CompletedTask;
        var windowData = OpenWindow(initWindow);
        var openTask = transition != null
            ? transition.Open(windowData.rectTransform, windowData.canvasGroup)
            : Task.CompletedTask;
        
        await Task.WhenAll(closeTask, openTask);
        if (_windows.Count > 2)
        {
            HideWindow(_windows.Count - 2, deactivateLastWindow);
        }

        return windowData.mediator as TMediator;
    }
    
    public async Task<bool> CloseWindowAsync<TMediator>() where TMediator : class, IMediator
    {
        for (var i = 0; i < _windows.Count; i++)
        {
            if (_windows[i].mediator.GetType() != typeof(TMediator)) 
                continue;

            var closingWindows = _windows[i];
            var openingWindow = i == _windows.Count - 1 ? default : _windows[^1];
            var result = await CloseWindowAsync(closingWindows, openingWindow, i, _windowTransition);

            return result;
        }
        return false;
    }

    public async Task<bool> CloseWindowAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator
    {
        for (var i = 0; i < _windows.Count; i++)
        {
            if (_windows[i].mediator != mediator) 
                continue;

            var closingWindows = _windows[i];
            var openingWindow = i == _windows.Count - 1 ? default : _windows[^1];
            var result = await CloseWindowAsync(closingWindows, openingWindow, i, _windowTransition);

            return result;
        }
        return false;
    }
    
    private async Task<bool> CloseWindowAsync(WindowData closingWindow, WindowData openingWindow, int closingWindowIndex, IWindowTransition transition)
    {
        var closeTask = closingWindow.mediator != null && transition != null
            ? transition.Close(_windows[^1].rectTransform, _windows[^1].canvasGroup)
            : Task.CompletedTask;
        var openTask = openingWindow.mediator != null && transition != null
            ? transition.Open(openingWindow.rectTransform, openingWindow.canvasGroup)
            : Task.CompletedTask;
        
        await Task.WhenAll(closeTask, openTask);
        openingWindow.mediator?.SetActive(true);
        CloseWindow(closingWindowIndex);

        return true;
    }

    #endregion
    
    private WindowData OpenWindow<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
#if DEVELOPMENT_BUILD
        var openingTime = new Stopwatch();
        openingTime.Start();
#endif
        
        if (_windowFactory.TryCreateWindow<TMediator>(_root, out var mediator, out var window) == false)
            return default;

#if RELEASE_BUILD
       try
        { 
#endif
        
            mediator.SetActive(true);
            mediator.OnInitialize();
            mediator.OnShow();
            initWindow?.Invoke(mediator);

#if RELEASE_BUILD
        }
        catch (Exception e)
        {
            Log.WriteError(e.Message);
        }
#endif
        var windowData = new WindowData
        {
            mediator = mediator,
            rectTransform = window.GetComponent<RectTransform>(),
            canvasGroup = window.GetComponent<CanvasGroup>(),
        };
            
        _windows.Add(windowData);
        
#if DEVELOPMENT_BUILD
        openingTime.Stop();
        //Log.WriteInfo($"Window opening time {mediator.GetType().Name}: {openingTime.Elapsed:mm\\:ss\\:ff}");
#endif

        return windowData;
    }

    private void CloseWindow(int index)
    {
        if (_windows.ElementAtOrDefault(index).mediator == null)
            return;
        
#if RELEASE_BUILD
       try
        { 
#endif
        
        _windows[index].mediator.OnHide();
        _windows[index].mediator.OnDestroy();
        _windows[index].mediator.Destroy();
    
        _windows.RemoveAt(index);

        if (_windows.ElementAtOrDefault(index - 1).mediator == null) 
            return;
        
#if RELEASE_BUILD
            
        }
        catch (Exception e)
        {
           Log.WriteError(e.Message);
        }
        try
        { 
#endif
        
       _windows[index - 1].mediator.SetActive(true);
       _windows[index - 1].mediator.OnShow();
        
#if RELEASE_BUILD
        }
        catch (Exception e)
        {
           Log.WriteError(e.Message);
        }
#endif
    }
    
    private void HideWindow(int index, bool deactivateLastWindow)
    {
        if (_windows.ElementAtOrDefault(index).mediator == null)
            return;
        
        _windows[index].mediator.OnHide();
        if (deactivateLastWindow)
            _windows[index].mediator.SetActive(false);
    }

    private struct WindowData
    {
        public IMediator mediator;
        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;
    }
}
}