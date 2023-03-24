using System;
using TMPro;
using UnityEngine;
using WarehouseKeeper.Gui.Windows;
using WarehouseKeeper.UI.Windows.ConfirmationWindows.Components;

namespace WarehouseKeeper.UI.Windows.ConfirmationWindows
{
internal class ConfirmWindowView : WindowUI
{
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _agreementText;
    [SerializeField] private TextMeshProUGUI _disagreementText;

    [SerializeField] private ConfirmWindowButton[] _windowButtons;

    public event Action<ConfirmWindowAction> OnWindowAction;

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
    
    private void ClickWindowAction(ConfirmWindowAction action) => OnWindowAction?.Invoke(action);

#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        _windowButtons = GetComponentsInChildren<ConfirmWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
    
    public void Setup(string descriptionText, string agreeText, string disagreeText)
    {
        _description.text = descriptionText;
        _agreementText.text = agreeText;
        _disagreementText.text = disagreeText;
    }
}
}
