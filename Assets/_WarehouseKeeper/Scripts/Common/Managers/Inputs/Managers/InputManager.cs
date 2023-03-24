using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WarehouseKeeper.Extension;
using Zenject;

namespace WarehouseKeeper.Inputs.Implementation
{
internal class InputManager : ITickable, IInputManager
{
    public event Action<Vector2, bool> OnStartInput;
    public event Action<Vector2, bool> OnStayInput;
    public event Action<Vector2, bool> OnEndInput;

    //    (0, Screen.height) Y      (Screen.width, Screen.height)
    //                       | -  -  -  -  -  -  -
    //                       |                   |
    //                       |                   
    //                       |                   |
    //                       +--------------------->
    //                    (0, 0)          (Screen.width, 0)
    public void Tick()
    {
        const int buttonId = 0;
        if (Input.GetMouseButtonDown(buttonId))
        {
            OnStartInput?.Invoke(Input.mousePosition, IsGuiClick(Input.mousePosition));
        }
        
        if (Input.GetMouseButton(buttonId))
        {
            OnStayInput?.Invoke(Input.mousePosition, false);
        }
        
        if (Input.GetMouseButtonUp(buttonId))
        {
            OnEndInput?.Invoke(Input.mousePosition, IsGuiClick(Input.mousePosition));
        }
    }

    private bool IsGuiClick(Vector3 screenPos)
    {
        var currentEventSystem = EventSystem.current;
        var tempRaycastResults = new List<RaycastResult>(10);

        if (currentEventSystem != null)
        {
            var tempPointerEventData = new PointerEventData(currentEventSystem) {position = screenPos};

            currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

            // Loop through all results and remove any that don't match the layer mask
            // if (tempRaycastResults.Count > 0)
            // {
            //     for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
            //     {
            //         var raycastResult = tempRaycastResults[i];
            //         var raycastLayer  = 1 << raycastResult.gameObject.layer;
            //
            //         if ((raycastLayer & layerMask) == 0)
            //         {
            //             tempRaycastResults.RemoveAt(i);
            //         }
            //     }
            // }
        }
        else
        {
            Log.WriteError("Failed to RaycastGui because your scene doesn't have an event system! To add one, go to: GameObject/UI/EventSystem");
        }

        return tempRaycastResults.Count > 0;
    }
}
}