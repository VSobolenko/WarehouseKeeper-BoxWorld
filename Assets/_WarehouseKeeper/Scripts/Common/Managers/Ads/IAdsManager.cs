using System;
using System.Threading.Tasks;

namespace WarehouseKeeper.Directors.Common.Managers.Ads
{
public interface IAdsManager
{
    public void InitSdk();
    public void SetUserId(string userId);
    public void ShowDebugger();
    
    //Rewarded
    public bool ReadyToWatchRewarded { get; }
    public event Action OnReadyToWatchRewarded;
    public Task<ViewResult> ShowRewardedAd();

    //Interstitial
    public bool ReadyToWatchInterstitial { get; }
    public event Action OnReadyToWatchInterstitial;
    public void ShowInterstitialAd();

}

public struct ViewResult
{
    public bool success;
}
}