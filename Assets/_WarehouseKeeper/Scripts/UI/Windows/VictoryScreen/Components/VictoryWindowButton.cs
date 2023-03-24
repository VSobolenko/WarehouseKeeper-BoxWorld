using WarehouseKeeper.UI.Windows;

namespace WarehouseKeeper.Directors.UI.Windows.VictoryScreen.Components
{
public class VictoryWindowButton : BaseButton<VictoryWindowAction>
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