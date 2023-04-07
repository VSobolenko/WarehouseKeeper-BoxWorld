using Game.GUI.Windows.Components;

namespace WarehouseKeeper.UI.Windows.SettingsWindows.SubWindows.Elements
{
internal class LanguageSelectionWindowButton : BaseButton<LanguageSelectionWindowAction>
{
#if UNITY_EDITOR

    [UnityEngine.ContextMenu("Editor simulate click")]
    private void EditorClickSimulate()
    {
        configuration.SimulateClick();
    }
    
    [UnityEngine.ContextMenu("Editor force validate")]
    private void EditorForceValidate()
    {
        configuration?.ValidateButton(transform);
    }
    
#endif
}
}