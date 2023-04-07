using System;
using Game;
using UnityEngine;

namespace WarehouseKeeper.UI.Windows
{
public class StarsView : MonoBehaviour
{
    [SerializeField] private StarData[] _stars;

    public void StaticSetup(int countReceived)
    {
#if UNITY_EDITOR
        if (_stars.Length <= 0)
            Log.Warning("Null star array");
#endif
        
        for (var i = 0; i < _stars.Length; i++)
        {
            _stars[i].foreground.SetActive(i < countReceived);
        }
    }

    public void SetActive(bool value) => gameObject.SetActive(value);
    
    [Serializable]
    private struct StarData
    {
        public GameObject root;
        public GameObject background;
        public GameObject foreground;
    }
}
}