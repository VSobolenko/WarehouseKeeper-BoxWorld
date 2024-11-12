using System;
using System.Threading.Tasks;
using Game.GUI.Windows;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.LevelSelectionWindows.SybWindows;
using WarehouseKeeper.Directors.UI.Windows.VictoryScreen;
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
    private readonly IWindowsManagerAsync _windowsManager;

    public WindowsDirector(IWindowsManagerAsync windowsManager)
    {
        _windowsManager = windowsManager;
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
    
    internal Task<LevelSelectionWindowMediator> OpenLevelSelectionWindow()
    {
        return _windowsManager.OpenWindowOnTopAsync<LevelSelectionWindowMediator>();
    }
    
    internal Task<ShopWindowMediator> OpenShopWindow()
    {
        return _windowsManager.OpenWindowOnTopAsync<ShopWindowMediator>();
    }
    
    internal Task<SettingsWindowMediator> OpenSettingsWindow()
    {
        return _windowsManager.OpenWindowOnTopAsync<SettingsWindowMediator>();
    }
    
    internal AppearanceWindowMediator OpenAppearanceWindow()
    {
        return _windowsManager.OpenWindowOnTop<AppearanceWindowMediator>();
    }
    
    internal ConfirmWindowMediator OpenConfirmWindow(Action<ConfirmWindowMediator> initWindow)
    {
        return _windowsManager.OpenWindowOnTop(initWindow);
    }
    
    internal Task<LanguageSelectionWindowMediator> OpenLanguageSelectionWindow()
    {
        return _windowsManager.OpenWindowOnTopAsync<LanguageSelectionWindowMediator>();
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
    internal void CloseWindowAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator => _windowsManager.CloseWindowAsync(mediator);
    
    internal void CloseWindow<TMediator>() where TMediator : class, IMediator => _windowsManager.CloseWindow<TMediator>();
    
    internal void CloseWindows() => _windowsManager.CloseWindows();
}
}