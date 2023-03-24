using System;
using UnityEngine;
using WarehouseKeeper.Directors.Common.Managers.Ads;
using Zenject;

namespace WarehouseKeeper.Directors.Game.Ads
{
public class AdsDebugger : MonoBehaviour
{
    
#if DEVELOPMENT_BUILD
    [SerializeField] private int _countTouch = 4;
    [SerializeField] private KeyCode _standaloneKey = KeyCode.LeftAlt;

    [Inject] private IAdsManager _adsManager;
    private void Update()
    {
        if (Input.touchCount == _countTouch || Input.GetKeyDown(_standaloneKey))
            _adsManager?.ShowDebugger();
    }

#endif
}
}