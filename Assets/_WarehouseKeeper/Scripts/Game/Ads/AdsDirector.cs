using Game.Ads;
using WarehouseKeeper.Directors.Game.UserResources;

namespace WarehouseKeeper.Directors.Game.Ads
{
internal class AdsDirector
{
    private readonly IAdsManager _adsManager;
    private readonly PlayerResourcesDirector _playerResourcesDirector;

    private const int AmountEnjoyment = 3;
    private int _sessionView;
    
    public AdsDirector(IAdsManager adsManager, PlayerResourcesDirector playerResourcesDirector)
    {
        _adsManager = adsManager;
        _playerResourcesDirector = playerResourcesDirector;
    }

    public void TryShowAd()
    {
        if (_playerResourcesDirector?.UserData == null || _playerResourcesDirector.UserData.AdsDisable)
            return;
        _sessionView++;
        if (_sessionView % AmountEnjoyment != 0)
            return;
        _sessionView = 0;

        _adsManager.ShowInterstitialAd();
    }
}
}