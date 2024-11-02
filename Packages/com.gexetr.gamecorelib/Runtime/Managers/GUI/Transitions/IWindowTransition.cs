using System.Threading.Tasks;
using Game.GUI.Windows.Managers;

namespace Game.GUI.Windows.Transitions
{
public interface IWindowTransition
{
    Task Open(WindowProperties windowProperties);
    Task Close(WindowProperties windowProperties);
}
}