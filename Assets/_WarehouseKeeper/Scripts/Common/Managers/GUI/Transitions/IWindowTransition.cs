using System.Threading.Tasks;
using UnityEngine;

namespace WarehouseKeeper.Gui.Windows.Transitions
{
internal interface IWindowTransition
{
    Task Open(RectTransform transform, CanvasGroup canvasGroup);
    Task Close(RectTransform transform, CanvasGroup canvasGroup);
}
}