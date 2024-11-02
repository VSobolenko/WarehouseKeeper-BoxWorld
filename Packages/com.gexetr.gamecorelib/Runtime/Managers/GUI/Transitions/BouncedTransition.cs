using System.Threading.Tasks;
using DG.Tweening;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
internal class BouncedTransition : IWindowTransition
{
    private readonly WindowSettings _settings;
    private readonly int _width;
    private readonly int _height;

    public BouncedTransition(WindowSettings settings)
    {
        _settings = settings;
        _width = Screen.width;
        _height = Screen.height;
    }

    public Task Open(WindowProperties windowProperties)
    {
        var completionSource = new TaskCompletionSource<bool>();

        var transform = windowProperties.rectTransform;
        windowProperties.mediator.SetInteraction(false);
        
        transform.localScale = Vector3.zero;
        BounceWindow(transform, Vector3.one, _settings.TransitionMoveDuration / 2f, () =>
        {
            windowProperties.mediator.SetInteraction(true);
            completionSource.SetResult(true);
        });

        return completionSource.Task;
    }

    public Task Close(WindowProperties windowProperties)
    {
        var completionSource = new TaskCompletionSource<bool>();

        var transform = windowProperties.rectTransform;
        var activePos = transform.localPosition;

        windowProperties.mediator.SetInteraction(false);

        BounceWindow(transform, Vector3.zero, 0, () =>
        {
            windowProperties.mediator.SetInteraction(true);
            completionSource.SetResult(true);
            transform.localPosition = activePos;
        });

        return completionSource.Task;
    }

    private void BounceWindow(Transform transform, Vector3 to, float startDelay, TweenCallback completeAction)
    {
        var seq = DOTween.Sequence();
        seq.PrependInterval(startDelay)
           .Append(transform.DOScale(to, _settings.TransitionMoveDuration / 2f)
                            .SetEase(_settings.MoveType))
           .OnComplete(completeAction);
    }
}
}