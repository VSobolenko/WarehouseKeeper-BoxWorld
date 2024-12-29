using System;
using System.Threading.Tasks;

namespace Game.Ads
{
internal class AdsPlug : IAdsManager
{
    public void InitSdk()
    {
        
    }

    public void SetUserId(string userId)
    {
    }

    public void ShowDebugger()
    {
    }

    public bool ReadyToWatchRewarded => false;
    public event Action OnReadyToWatchRewarded;

    public Task<ViewResult> ShowRewardedAd()
    {
        return Task.FromResult(new ViewResult
        {
            success = false,
        });
    }

    public bool ReadyToWatchInterstitial => false;
    public event Action OnReadyToWatchInterstitial;
    public void ShowInterstitialAd()
    {
    }

    private void WarningDisable()
    {
        OnReadyToWatchRewarded?.Invoke();
        OnReadyToWatchInterstitial?.Invoke();
    }
}
}