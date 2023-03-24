using UnityEditor;
using UnityEngine;

namespace WarehouseKeeper.EditorTools.Levels.HintEditorHelper
{
[CustomEditor(typeof(HintTunel))]
public class HintTunelProvider : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("CLEAR"))
        {
            var tunel = (HintTunel) target;
            if (tunel == null)
            {
                Debug.Log("Cannot cast target");
                return;
            }
            tunel.ClearHints();
        }
        base.OnInspectorGUI();
    }
}
}