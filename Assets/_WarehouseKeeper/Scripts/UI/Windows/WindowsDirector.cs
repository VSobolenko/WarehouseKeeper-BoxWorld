using System;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.LevelSelectionWindows.SybWindows;
using WarehouseKeeper.Directors.UI.Windows.VictoryScreen;
using WarehouseKeeper.Gui.Windows;
using WarehouseKeeper.Gui.Windows.Mediators;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows.AppearanceWindows;
using WarehouseKeeper.UI.Windows.ConfirmationWindows;
using WarehouseKeeper.UI.Windows.GameWindows;
using WarehouseKeeper.UI.Windows.LevelSelections;
using WarehouseKeeper.UI.Windows.MainWindows;
using WarehouseKeeper.UI.Windows.SettingsWindows;
using WarehouseKeeper.UI.Windows.SettingsWindows.SubWindows;
using WarehouseKeeper.UI.Windows.ShopWindows;
using Zenject;

namespace WarehouseKeeper.Directors.UI.Windows
{
internal class WindowsDirector : IInitializable
{
    private readonly IWindowsManager _windowsManager;
    private readonly IWindowsManagerAsync _windowsManagerAsync;

    public WindowsDirector(IWindowsManager windowsManager, IWindowsManagerAsync windowsManagerAsync)
    {
        _windowsManager = windowsManager;
        _windowsManagerAsync = windowsManagerAsync;
    }

    public void Initialize()
    {
        OpenMainWindow();
    }

    internal TMediator GetFirstOrDefaultWindow<TMediator>() where TMediator : class, IMediator
    {
        _windowsManager.TryGetActiveWindow<TMediator>(out var mediator);

        return mediator;
    }
    
    internal MainWindowMediator OpenMainWindow()
    {
        return _windowsManager.OpenWindowOnTop<MainWindowMediator>();
    }
    
    internal GameWindowMediator OpenGameWindow(Action<GameWindowMediator> initWindow = null)
    {
        return _windowsManager.OpenWindowOnTop(initWindow);
    }
    
    internal LevelSelectionWindowMediator OpenLevelSelectionWindow()
    {
        return _windowsManager.OpenWindowOnTop<LevelSelectionWindowMediator>();
    }
    
    internal ShopWindowMediator OpenShopWindow()
    {
        return _windowsManager.OpenWindowOnTop<ShopWindowMediator>();
    }
    
    internal SettingsWindowMediator OpenSettingsWindow()
    {
        return _windowsManager.OpenWindowOnTop<SettingsWindowMediator>();
    }
    
    internal AppearanceWindowMediator OpenAppearanceWindow()
    {
        return _windowsManager.OpenWindowOnTop<AppearanceWindowMediator>();
    }
    
    internal ConfirmWindowMediator OpenConfirmWindow(Action<ConfirmWindowMediator> initWindow)
    {
        return _windowsManager.OpenWindowOnTop(initWindow);
    }
    
    internal LanguageSelectionWindowMediator OpenLanguageSelectionWindow()
    {
        return _windowsManager.OpenWindowOnTop<LanguageSelectionWindowMediator>();
    }
    
    internal VictoryWindowMediator OpenVictoryWindow(int levelID, LevelStatistics statistics)
    {
        return _windowsManager.OpenWindowOnTop<VictoryWindowMediator>(window =>
        {
            window.Setup(levelID, statistics);
        });
    }
    
    internal LevelInfoWindowMediator OpenLevelInfoWindow(int levelID)
    {
        return _windowsManager.OpenWindowOnTop<LevelInfoWindowMediator>(window =>
        {
            window.Setup(levelID);
        });
    }
    
    internal void CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator => _windowsManager.CloseWindow(mediator);
    
    internal void CloseWindow<TMediator>() where TMediator : class, IMediator => _windowsManager.CloseWindow<TMediator>();
    
    internal void CloseWindows() => _windowsManager.CloseWindows();
}
}