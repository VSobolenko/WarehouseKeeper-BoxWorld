using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Gui.Windows.Transitions
{
internal class VerticalTransition : IWindowTransition
{
    private readonly WindowSettings _settings;
    private readonly int _width;
    private readonly int _height;

    public VerticalTransition(WindowSettings settings)
    {
        _settings = settings;
        _width = Screen.width;
        _height = Screen.height;
    }

    public Task Open(RectTransform transform, CanvasGroup canvasGroup)
    {
        var completionSource = new TaskCompletionSource<bool>();

#if DEVELOPMENT_BUILD
        if (transform == null || canvasGroup == null)
            Log.WriteError("Null transition components");
#endif
        
        canvasGroup.blocksRaycasts = false;

        var activePos = transform.localPosition;
        var startPos = new Vector3(activePos.x, activePos.x + _height, activePos.z);

        transform.localPosition = startPos;
        var targetHeight = transform.rect.height + transform.rect.height / 2 * Vector3.down.y;

        transform.DOMoveY(targetHeight, _settings.TransitionMoveDuration)
                 .SetEase(_settings.MoveType)
                 .OnComplete(() =>
                 {
                     canvasGroup.blocksRaycasts = true;
                     completionSource.SetResult(true);
                 });
        MoveWindow(transform, Vector3.down, () =>
        {
            canvasGroup.blocksRaycasts = true;
            completionSource.SetResult(true);
        });

        return completionSource.Task;
    }

    public Task Close(RectTransform transform, CanvasGroup canvasGroup)
    {
        var completionSource = new TaskCompletionSource<bool>();
        canvasGroup.blocksRaycasts = false;
        var activePos = transform.localPosition;
        var startPos = new Vector3(activePos.x, transform.rect.height - _height, activePos.z);
        transform.localPosition = startPos;

        var targetHeight = transform.rect.height / 2 - transform.rect.height;

        transform.DOMoveY(targetHeight, _settings.TransitionMoveDuration)
                 .SetEase(_settings.MoveType)
                 .OnComplete(() =>
                 {
                     canvasGroup.blocksRaycasts = true;
                     completionSource.SetResult(true);
                 });

        return completionSource.Task;
    }

    private void MoveWindow(RectTransform transform, Vector3 direction, TweenCallback completeAction)
    {
        var targetHeight = transform.rect.height + transform.rect.height / 2 * direction.y;

        transform.DOMoveY(targetHeight, _settings.TransitionMoveDuration)
                 .SetEase(_settings.MoveType)
                 .OnComplete(completeAction);
    }
}
}