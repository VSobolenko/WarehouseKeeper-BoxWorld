using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Windows.Components
{
public class BaseButton<T> : MonoBehaviour where T : struct, Enum
{
    [SerializeField] private ButtonConfiguration<T> configuration;
    
    public event Action<T> OnClickButton;

    private void Start()
    {
        configuration.ObserveButton();
        configuration.OnClickButton += ClickButton;
    }

    private void ClickButton(T action) => OnClickButton?.Invoke(action);
    
    private void OnValidate() => configuration?.ValidateButton(transform);
    
    [ContextMenu("Editor simulate click")]
    protected void SimulateClick() => configuration.SimulateClick();
    
    [ContextMenu("Editor force validate")]
    protected void ForceValidate() => OnValidate();
}


[Serializable,]
public class ButtonConfiguration<T> where T : struct, Enum
{
    [SerializeField] private Button button;
    [SerializeField] private T action;

    public event Action<T> OnClickButton;

    internal void ObserveButton() => button.onClick.AddListener(ButtonClick);
    
    private void ButtonClick() => OnClickButton?.Invoke(action);

    public void ValidateButton(Transform root)
    {
        if (button == null) button = root.GetComponent<Button>();
    }
    
    public void SimulateClick()
    {
        Log.Info($"Invoke editor simulation action: {action}");
        ButtonClick();
    }
}
}