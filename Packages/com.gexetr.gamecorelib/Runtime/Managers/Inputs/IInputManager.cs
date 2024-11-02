using System;
using UnityEngine;

namespace Game.Inputs
{
public interface IInputManager : IUpdatable
{
    event Action<Vector2, bool> OnStartInput; 
    event Action<Vector2, bool> OnStayInput; 
    event Action<Vector2, bool> OnEndInput; 
}
}