using System;
using System.Threading.Tasks;

namespace Game.Ads
{
public interface IAdsManager
{
    void InitSdk();
    void SetUserId(string userId);
    void ShowDebugger();
    
    //Rewarded
    bool ReadyToWatchRewarded { get; }
    event Action OnReadyToWatchRewarded;
    Task<ViewResult> ShowRewardedAd();

    //Interstitial
    bool ReadyToWatchInterstitial { get; }
    event Action OnReadyToWatchInterstitial;
    void ShowInterstitialAd();

}

public struct ViewResult
{
    public bool success;
}
}