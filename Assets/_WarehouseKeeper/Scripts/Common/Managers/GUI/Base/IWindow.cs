namespace WarehouseKeeper.Gui.Windows
{
public interface IWindow
{
    void OnInitialize();
    void OnShow();
    void OnHide();
    void OnDestroy();
}
}