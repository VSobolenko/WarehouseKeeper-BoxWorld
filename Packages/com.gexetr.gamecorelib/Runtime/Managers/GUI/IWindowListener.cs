namespace Game.GUI.Windows
{
public interface IWindowListener
{
    void OnInitialize();
    void OnFocus();
    void OnUnfocused();
    void OnDestroy();
}
}