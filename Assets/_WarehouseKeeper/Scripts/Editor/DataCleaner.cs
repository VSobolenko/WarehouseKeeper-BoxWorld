using UnityEditor;

namespace WarehouseKeeper.EditorScripts
{
public class DataCleaner : GameDataCleaner
{
    [MenuItem(DefaultHeader, false)]
    private static void ShowWindow() => ShowDataCleanerWindow<DataCleaner>(startupConfigure: window =>
    {
        window.showHeader = true;
    });
}
}