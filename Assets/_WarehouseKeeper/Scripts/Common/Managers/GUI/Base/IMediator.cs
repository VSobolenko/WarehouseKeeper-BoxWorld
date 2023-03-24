namespace WarehouseKeeper.Gui.Windows.Mediators
{
internal interface IMediator : IWindow
{
    void SetActive(bool value);
    bool IsActive();
    void Destroy();
}
}