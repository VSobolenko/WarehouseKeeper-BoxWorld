using System;

namespace Game.GUI.Windows
{
public interface IWindowsManager : IWindowsContainer
{
    /// <summary>
    /// Opens a new window on top and does not hide the previous one
    /// </summary>
    /// <param name="initWindow"></param>
    /// <typeparam name="TMediator"></typeparam>
    /// <returns></returns>
    TMediator OpenWindowOnTop<TMediator>(Action<TMediator> initWindow = null) where TMediator : class, IMediator;

    /// <summary>
    /// Opens a new window on top and hides the previous one
    /// </summary>
    /// <param name="initWindow"></param>
    /// <typeparam name="TMediator"></typeparam>
    /// <returns></returns>
    TMediator OpenWindowOver<TMediator>(Action<TMediator> initWindow = null) where TMediator : class, IMediator;

    /// <summary>
    /// CLose first window by mediator type
    /// </summary>
    /// <typeparam name="TMediator"></typeparam>
    /// <returns></returns>
    bool CloseWindow<TMediator>() where TMediator : class, IMediator;
    
    /// <summary>
    /// Close first window by reference
    /// </summary>
    /// <param name="mediator"></param>
    /// <typeparam name="TMediator"></typeparam>
    /// <returns></returns>
    bool CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator;

    /// <summary>
    /// Close all window
    /// </summary>
    void CloseWindows();
}
}