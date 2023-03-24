using UnityEditor;
using UnityEngine;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Localizations.Components;

namespace WarehouseKeeper.EditorTools.Localizations
{
[CustomEditor(typeof(LocalizableTMP))]
public class LocalizableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Editor localize"))
        {
            var localizableBehaviour = (LocalizableTMP) target;
            if (localizableBehaviour == null)
            {
                Log.InternalError();
                return;
            }
            localizableBehaviour.EditorLocalize();
        }
    }
}
}