using System;
using System.Threading.Tasks;
using Game.GUI.Installers;
using Game.GUI.Windows;
using Game.GUI.Windows.Transitions;
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
    private readonly IWindowTransition _openBounced;
    private readonly IWindowTransition _closeBounced;

    public WindowsDirector(IWindowsManagerAsync windowsManager)
    {
        _windowsManager = windowsManager;
        _openBounced = GuiInstaller.Configurable(GuiInstaller.Bounced(), GuiInstaller.Empty(), false);
        _closeBounced = GuiInstaller.Configurable(GuiInstaller.Empty(), GuiInstaller.Bounced(), true, false);
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
    
    internal Task<ShopWindowMediator> OpenShopWindowAsync()
    {
        return _windowsManager.OpenWindowOnTopAsync<ShopWindowMediator>();
    }
    
    internal ShopWindowMediator OpenShopWindow(Action<ShopWindowMediator> action = null)
    {
        return _windowsManager.OpenWindowOnTop<ShopWindowMediator>(action);
    }
    
    internal Task<SettingsWindowMediator> OpenSettingsWindow()
    {
        return _windowsManager.OpenWindowOnTopAsync<SettingsWindowMediator>();
    }
    
    internal Task<AppearanceWindowMediator> OpenAppearanceWindow()
    {
        return _windowsManager.OpenWindowOnTopAsync<AppearanceWindowMediator>();
    }
    
    internal Task<ConfirmWindowMediator> OpenConfirmWindow(Action<ConfirmWindowMediator> initWindow)
    {
        return _windowsManager.OpenWindowOnTopAsync(_openBounced, initWindow);
    }
    
    internal Task<LanguageSelectionWindowMediator> OpenLanguageSelectionWindow()
    {
        return _windowsManager.OpenWindowOnTopAsync<LanguageSelectionWindowMediator>(_openBounced);
    }
    
    internal void OpenVictoryWindow(int levelID, LevelStatistics statistics)
    {
        _windowsManager.OpenWindowOnTopAsync<VictoryWindowMediator>(_openBounced, window =>
        {
            window.Setup(levelID, statistics);
        });
    }
    
    internal void OpenLevelInfoWindow(int levelID)
    {
        _windowsManager.OpenWindowOnTopAsync<LevelInfoWindowMediator>(_openBounced, window =>
        {
            window.Setup(levelID);
        });
    }
    
    internal void CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator => _windowsManager.CloseWindow(mediator);
    internal void CloseWindowAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator => _windowsManager.CloseWindowAsync(mediator);
    internal void CloseWindowBouncedAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator => _windowsManager.CloseWindowAsync(_closeBounced, mediator);
    
    internal void CloseWindow<TMediator>() where TMediator : class, IMediator => _windowsManager.CloseWindow<TMediator>();
    
    internal void CloseWindows() => _windowsManager.CloseWindows();
}
}