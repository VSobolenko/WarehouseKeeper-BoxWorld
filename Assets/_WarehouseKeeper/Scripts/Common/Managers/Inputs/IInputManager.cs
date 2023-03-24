using System;
using UnityEngine;

namespace WarehouseKeeper.Inputs
{
public interface IInputManager
{
    event Action<Vector2, bool> OnStartInput; 
    event Action<Vector2, bool> OnStayInput; 
    event Action<Vector2, bool> OnEndInput; 
}
}