using WarehouseKeeper.Gui.Windows.Mediators;

namespace WarehouseKeeper.Gui.Windows
{
internal interface IWindowsContainer
{
    /// <summary>
    /// Returns a list of available windows by type, if there is more than one such type
    /// </summary>
    /// <param name="mediator"></param>
    /// <typeparam name="TMediator"></typeparam>
    /// <returns></returns>
    public bool TryGetActiveWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator;
    
    /// <summary>
    /// Returns the first available window by mediator type
    /// </summary>
    /// <param name="mediator"></param>
    /// <typeparam name="TMediator"></typeparam>
    /// <returns></returns>
    public bool TryGetActiveWindow<TMediator>(out TMediator mediator) where TMediator : class, IMediator;
}
}