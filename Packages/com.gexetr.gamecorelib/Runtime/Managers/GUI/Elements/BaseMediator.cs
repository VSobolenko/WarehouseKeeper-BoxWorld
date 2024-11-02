using UnityEngine;

namespace Game.GUI.Windows
{
/// <summary>
/// Base class for window mediators
/// Method execution order:
/// +--------------------------------+
/// | SetActive()
/// |
/// | OnInitialize()
/// |  OnFocus()
/// |  InitAction.Invoke()
/// |  ...
/// |    SetActive()
/// |    OnFocus() / OnUnfocused()
/// |  ...
/// |  OnUnfocused()
/// |  OnDestroy()
/// |
/// |  Destroy()
/// +--------------------------------+
/// </summary>
/// <typeparam name="TWindow"></typeparam>
public abstract class BaseMediator<TWindow> : IMediator where TWindow : WindowUI
{
    protected readonly TWindow window;

    protected BaseMediator(TWindow window)
    {
        this.window = window;
    }

    public virtual void OnInitialize() { }
    public virtual void OnFocus() { }
    public virtual void OnUnfocused() { }
    public virtual void OnDestroy() { }

    public void SetActive(bool value) => window.gameObject.SetActive(value);
    public void SetInteraction(bool value) => window.canvasGroup.blocksRaycasts = value;
    public bool IsActive() => window.gameObject.activeInHierarchy;
    public void Destroy() => Object.Destroy(window.gameObject);
}
}