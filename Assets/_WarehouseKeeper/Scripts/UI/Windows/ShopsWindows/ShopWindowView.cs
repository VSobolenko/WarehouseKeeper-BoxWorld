using System;
using Game.GUI.Windows;
using UnityEngine;
using WarehouseKeeper.UI.Windows.ShopWindows.Components;

namespace WarehouseKeeper.UI.Windows.ShopWindows
{
internal sealed class ShopWindowView : WindowUI
{
    [field: SerializeField] public RectTransform ProductsRoot { get; private set; }
    [field: SerializeField] public PlayerResourcesView PlayerResourcesView { get; private set; }
    [field: SerializeField] public InformedText Informer { get; private set; }
    [SerializeField] private CanvasGroup _selfCanvasGroup;
    [SerializeField] private ShopWindowButton[] _windowButtons;

    public event Action<ShopWindowAction> OnWindowAction;

    private void Start()
    {
        foreach (var button in _windowButtons)
            button.OnClickButton += ClickWindowAction;
    }

    private void OnDestroy()
    {
        foreach (var button in _windowButtons)
            button.OnClickButton -= ClickWindowAction;
    }

    private void ClickWindowAction(ShopWindowAction action)
    {
        OnWindowAction?.Invoke(action);
    }
    
#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        _windowButtons = GetComponentsInChildren<ShopWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
    public void DisableInteraction() => _selfCanvasGroup.interactable = false;

    public void EnableInteraction() => _selfCanvasGroup.interactable = true;
}
}