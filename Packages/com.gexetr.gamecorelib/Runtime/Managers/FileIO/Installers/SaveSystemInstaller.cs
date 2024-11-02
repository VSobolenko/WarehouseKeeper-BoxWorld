using Game.IO.Managers;

namespace Game.IO.Installers
{
public class SaveSystemInstaller
{
    public static ISaveFile FileSaver() => new BinarySave();
}
}