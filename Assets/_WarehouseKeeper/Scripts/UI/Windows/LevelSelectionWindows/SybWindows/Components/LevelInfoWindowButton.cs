using Game.GUI.Windows.Components;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.LevelSelectionWindows.SybWindows.Components
{
public class LevelInfoWindowButton : BaseButton<LevelInfoWindowAction>
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