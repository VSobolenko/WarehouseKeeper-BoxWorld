using System;
using UnityEngine;

namespace Game.Inputs
{
public class SwipeDetector : IDisposable
{
    private readonly IInputManager _inputManager;
    private readonly InputSettings _settings;
    
    private Vector2 _endPosition;
    private Vector2 _startPosition;
    private DateTime _startTime;
    private bool _startInUI = false;
    
    public SwipeDetector(IInputManager inputManager, InputSettings settings)
    {
        _inputManager = inputManager;
        _settings = settings;

        _inputManager.OnStartInput += SwipeStart;
        _inputManager.OnEndInput += SwipeEnd;
    }

    public void Dispose()
    {
        _inputManager.OnStartInput -= SwipeStart;
        _inputManager.OnEndInput -= SwipeEnd;
    }

    public event Action<Vector2, Vector2> OnSwipe;
    public event Action<Vector2> OnSwipeNormalized;

    private void SwipeStart(Vector2 position, bool isUIElement)
    {
        _startPosition = position;
        _startTime = DateTime.Now;
        _startInUI = isUIElement;
    }

    private void SwipeEnd(Vector2 position, bool isUIElement)
    {
        _endPosition = position;
        var isUIClick = _startInUI && isUIElement;
        if (isUIClick)
            return;
        DetectSwipe((DateTime.Now - _startTime).TotalSeconds);
    }

    private void DetectSwipe(double swipeTime)
    {
        if (swipeTime > _settings.MaxSwipeTimeSeconds)
            return;
        
        if (Vector2.Distance(_startPosition, _endPosition) < _settings.SwipeMinimumDistance)
            return;
        
        Vector3 direction = _endPosition - _startPosition;
        if (IsSwipe(direction.normalized, out var swipeDirection) == false)
            return;
        OnSwipe?.Invoke(_startPosition, _endPosition); 
        OnSwipeNormalized?.Invoke(swipeDirection);
    }

    private bool IsSwipe(Vector2 direction, out Vector2 swipeDirection)
    {
        if (Vector2.Dot(Vector2.up, direction) > _settings.DirectionThreshold)
        {
            swipeDirection = Vector2.up;

            return true;
        }

        if (Vector2.Dot(Vector2.down, direction) > _settings.DirectionThreshold)
        {
            swipeDirection = Vector2.down;

            return true;
        }

        if (Vector2.Dot(Vector2.right, direction) > _settings.DirectionThreshold)
        {
            swipeDirection = Vector2.right;

            return true;
        }

        if (Vector2.Dot(Vector2.left, direction) > _settings.DirectionThreshold)
        {
            swipeDirection = Vector2.left;

            return true;
        }

        swipeDirection = Vector2.zero;

        return false;
    }
}
}