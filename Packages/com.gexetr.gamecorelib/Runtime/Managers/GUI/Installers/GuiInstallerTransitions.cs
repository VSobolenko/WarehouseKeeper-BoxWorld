using Game.GUI.Windows.Transitions;

namespace Game.GUI.Installers
{
public static partial class GuiInstaller
{
    public static IWindowTransition VerticalTransition() => new VerticalTransition(WindowSettings);
    public static IWindowTransition HorizontalTransition() => new HorizontalTransition(WindowSettings);
    public static IWindowTransition BouncedTransition() => new BouncedTransition(WindowSettings);
}
}