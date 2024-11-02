using Game.GUI.Windows.Components;
using UnityEngine;

namespace Game.GUI.Windows
{
public abstract class BaseReactiveMediator<TWindow, TReactive> : IMediator 
    where TWindow : WindowUI 
    where TReactive : struct, System.Enum
{
    protected readonly TWindow window;
    protected readonly BaseButton<TReactive>[] reactiveButton;

    protected BaseReactiveMediator(TWindow window, BaseButton<TReactive>[] reactiveButton)
    {
        this.window = window;
        this.reactiveButton = reactiveButton;
    }

    public virtual void OnInitialize()
    {
        foreach (var button in reactiveButton)
        {
            button.OnClickButton += ProceedButtonAction;
        }
    }

    public virtual void OnFocus() { }

    public virtual void OnUnfocused() { }

    public virtual void OnDestroy()
    {
        foreach (var button in reactiveButton)
        {
            button.OnClickButton -= ProceedButtonAction;
        }
    }

    protected virtual void ProceedButtonAction(TReactive action) { }
    
    public void SetActive(bool value) => window.gameObject.SetActive(value);
    
    public void SetInteraction(bool value) => window.canvasGroup.blocksRaycasts = value;

    public bool IsActive() => window.gameObject.activeInHierarchy;

    public void Destroy() => Object.Destroy(window.gameObject);
}
}