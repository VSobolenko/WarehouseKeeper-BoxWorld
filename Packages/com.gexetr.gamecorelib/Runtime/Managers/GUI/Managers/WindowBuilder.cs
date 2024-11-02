using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI.Windows.Managers
{
internal class WindowBuilder : IDisposable, IEnumerable<WindowProperties>
{
    public int Count => _windows.Count;

    private readonly List<WindowProperties> _windows = new(8);
    private readonly IWindowFactory _windowFactory;
    private readonly Transform _root;

    public WindowBuilder(IWindowFactory windowFactory, Transform root)
    {
        _windowFactory = windowFactory;
        _root = root;
    }

    public void Dispose()
    {
        for (var i = _windows.Count - 1; i >= 0; i--)
        {
            CloseWindow(i);
        }

        _windows.Clear();
    }

    public WindowProperties OpenWindowSilently<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        if (_windowFactory.TryCreateWindow<TMediator>(_root, out var mediator, out var window) == false)
            return default;

        mediator.SetActive(true);
        mediator.SetInteraction(true);
        mediator.OnInitialize();
        mediator.OnFocus();
        initWindow?.Invoke(mediator);

        var windowData = new WindowProperties
        {
            mediator = mediator,
            rectTransform = (RectTransform) window.transform,
            mode = OpenMode.Silently,
        };

        var lastWindowData = _windows.Count > 0 ? _windows[^1] : windowData;
        _windows.Add(windowData);

        if (lastWindowData != windowData)
        {
            lastWindowData.mediator.OnUnfocused();
            lastWindowData.mediator.SetInteraction(false);
            lastWindowData.mediator.SetActive(windowData.mode == OpenMode.Silently);
        }

        return windowData;
    }

    public void CloseWindow(int index)
    {
        if (_windows.ElementAtOrDefault(index) == null)
            return;

        _windows[index].mediator.OnUnfocused();
        _windows[index].mediator.OnDestroy();
        _windows[index].mediator.Destroy();

        _windows.RemoveAt(index);

        if (_windows.ElementAtOrDefault(index - 1) == null)
            return;

        //Имеется общее
        _windows[index - 1].mediator.SetActive(true);
        _windows[index - 1].mediator.SetInteraction(true);
        _windows[index - 1].mediator.OnFocus();
        RestoreVisibilityMode();
    }

    private void RestoreVisibilityMode()
    {
        if (_windows.Count == 1)
            return;
        for (var i = _windows.Count - 2; i >= 0; i--)
        {
            var parentWindowData = _windows[i + 1];

            _windows[i].mediator.SetActive(parentWindowData.mode == OpenMode.Silently);
            if (parentWindowData.mode == OpenMode.Overlay)
                break;
        }
    }
    
    public void HideWindow(int index, bool deactivateLastWindow)
    {
        if (_windows.ElementAtOrDefault(index)?.mediator == null)
            return;

        _windows[index].mediator.OnUnfocused();
        if (deactivateLastWindow)
            _windows[index].mediator.SetActive(false);
    }

    public WindowProperties this[int i] => _windows[i];

    IEnumerator<WindowProperties> IEnumerable<WindowProperties>.GetEnumerator() => _windows.GetEnumerator();

    public IEnumerator GetEnumerator() => _windows.GetEnumerator();
}
}