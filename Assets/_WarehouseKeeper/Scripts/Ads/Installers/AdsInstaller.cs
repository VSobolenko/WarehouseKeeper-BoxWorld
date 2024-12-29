namespace Game.Ads.Installers
{
public static class AdsInstaller
{
    public static IAdsManager Plug() => new AdsPlug();
}
}