using System;
using UnityEngine;
using UnityEngine.UI;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.UI.Windows
{
public class BaseButton<T> : MonoBehaviour where T : struct, Enum
{
    [SerializeField] protected ButtonConfiguration<T> configuration;
    
    public event Action<T> OnClickButton;

    private void Start()
    {
        configuration.ObserveButton();
        configuration.OnClickButton += ClickButton;
    }

    private void ClickButton(T action) => OnClickButton?.Invoke(action);
    private void OnValidate() => configuration?.ValidateButton(transform);
}

[Serializable,]
public class ButtonConfiguration<T> where T : struct, Enum
{
    [SerializeField] private Button button;
    [SerializeField] private T action;

    public event Action<T> OnClickButton;

    public void ObserveButton() => button.onClick.AddListener(ButtonClick);
    
    private void ButtonClick() => OnClickButton?.Invoke(action);
    
    public void ValidateButton(Transform root)
    {
        if (button == null) button = root.GetComponent<Button>();
    }
    
#if UNITY_EDITOR

    internal void SimulateClick()
    {
        Log.WriteInfo($"Invoke editor simulation action: {action}");
        ButtonClick();
    }
    
#endif
}

}