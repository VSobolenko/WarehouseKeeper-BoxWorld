using System;
using UnityEngine;
using WarehouseKeeper.Gui.Windows;
using WarehouseKeeper.UI.Windows.AppearanceWindows.Components;
using WarehouseKeeper.UI.Windows.ShopWindows.Components;

namespace WarehouseKeeper.UI.Windows.AppearanceWindows
{
internal sealed class AppearanceWindowView : WindowUI
{
    [field: SerializeField] public RectTransform itemsRoot;
    [SerializeField] private GameObject buyRoot;
    [SerializeField] private GameObject selectRoot;
    [SerializeField] private GameObject receiveRoot;
    [SerializeField] private AppearanceWindowButton[] windowButtons;

    public event Action<AppearanceWindowAction> OnWindowAction;

    private void Start()
    {
        foreach (var button in windowButtons)
            button.OnClickButton += ClickWindowAction;
    }

    private void OnDestroy()
    {
        foreach (var button in windowButtons)
            button.OnClickButton -= ClickWindowAction;
    }

    private void ClickWindowAction(AppearanceWindowAction action)
    {
        OnWindowAction?.Invoke(action);
    }

    public void SetViewState(ItemState state)
    {
        switch (state)
        {
            case ItemState.Received:
                SetReceivedState();
                break;
            case ItemState.Selected:
                SetSelectedState();
                break;
            case ItemState.Blocked:
                SetBlockedState();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void SetReceivedState()
    {
        buyRoot.SetActive(false);
        selectRoot.SetActive(false);
        receiveRoot.SetActive(true);
    }

    private void SetSelectedState()
    {
        buyRoot.SetActive(false);
        selectRoot.SetActive(true);
        receiveRoot.SetActive(false);
    }

    private void SetBlockedState()
    {
        buyRoot.SetActive(true);
        selectRoot.SetActive(false);
        receiveRoot.SetActive(false);
    }
    
#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        windowButtons = GetComponentsInChildren<AppearanceWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
}
internal enum ItemState : byte
{
    Received,
    Selected,
    Blocked,
}
}