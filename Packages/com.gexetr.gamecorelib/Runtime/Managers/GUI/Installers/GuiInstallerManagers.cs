using Game.AssetContent;
using Game.Factories;
using Game.GUI.Windows;
using Game.GUI.Windows.Factories;
using Game.GUI.Windows.Managers;
using Game.GUI.Windows.Transitions;
using UnityEngine;

namespace Game.GUI.Installers
{
public static partial class GuiInstaller
{
    public static IWindowsManagerAsync ManagerAsync(IMediatorInstantiator mediatorBuilder, IResourceManagement resourceManagement,
                                                    IFactoryGameObjects factory, IWindowTransition transition,
                                                    Transform rootUi = null)
    {
        IWindowFactory windowFactory = new WindowsFactory(mediatorBuilder, resourceManagement, factory);

        return new WindowsManagerAsync(windowFactory, rootUi, WindowSettings, transition);
    }
}
}