using UnityEngine;

namespace WarehouseKeeper.Gui.Windows.Mediators
{
/// <summary>
/// Base class for windows
/// Method execution order:
///   SetActive()
///   OnInitialize()
///   OnShow()
///   InitAction.Invoke()
///   ...
///     SetActive()
///     OnShow() / OnHide()
///   ...
///   OnHide()
///   OnDestroy()
///   Destroy()
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

    public virtual void OnShow() { }

    public virtual void OnHide() { }

    public virtual void OnDestroy() { }

    public void SetActive(bool value) => window.gameObject.SetActive(value);

    public bool IsActive() => window.gameObject.activeInHierarchy;

    public void Destroy() => GameObject.Destroy(window.gameObject);
}
}