using Game.Factories.Managers;

namespace Game.Factories.Installers
{
public static class FactoryInstaller
{
    public static IFactoryGameObjects Standart() => new StandardObjectsFactory();
}
}