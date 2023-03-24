using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Directors.Common.Managers.Ads
{
internal class ApplovinAdManager : IAdsManager
{
    private readonly AdDetails _adDetails;

    #region APPLOVIN

    public ApplovinAdManager(AdDetails adDetails)
    {
        _adDetails = adDetails;
        InitSdk();
    }

    public bool ReadyToWatchRewarded => MaxSdk.IsRewardedAdReady(_adDetails.rewardedAdUnitId);

    public bool ReadyToWatchInterstitial => MaxSdk.IsInterstitialReady(_adDetails.interstitialAdUnitId);
    
    public event Action OnReadyToWatchInterstitial;

    public event Action OnReadyToWatchRewarded;

    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;

    private TaskCompletionSource<ViewResult> tcsShowAd;

    public void InitSdk()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.
            Log.Write("MAX SDK Initialized");

            InitializeInterstitialAds();
            InitializeRewardedAds();
        };

        MaxSdk.SetSdkKey(_adDetails.sdkKey);
        MaxSdk.InitializeSdk();
    }

    public void SetUserId(string userId)
    {
        MaxSdk.SetUserId(userId);
    }

    public void ShowDebugger() => MaxSdk.ShowMediationDebugger();

    private void InvokeOnReadyToWatchRewarded() => OnReadyToWatchRewarded?.Invoke();
    private void InvokeOnReadyToWatchInterstitial() => OnReadyToWatchInterstitial?.Invoke();

    #region Interstitial Ad Methods

    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        
        // Load the first interstitial
        LoadInterstitial();
    }

    void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(_adDetails.interstitialAdUnitId);
    }

    public void ShowInterstitialAd()
    {
        if (MaxSdk.IsInterstitialReady(_adDetails.interstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(_adDetails.interstitialAdUnitId);
        }
        else
        {
            Log.Write("Ad not ready");
        }
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        Log.Write("Interstitial loaded");
        
        // Reset retry attempt
        interstitialRetryAttempt = 0;
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        
        Log.Write("Interstitial failed to load with error code: " + errorInfo.Code);
        
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Log.Write("Interstitial failed to display with error code: " + errorInfo.Code);
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        Log.Write("Interstitial dismissed");
        LoadInterstitial();
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Log.Write("Interstitial revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;
        
        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
    }

    #endregion
    
    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        Log.Write("Loading rewarded ad");
        MaxSdk.LoadRewardedAd(_adDetails.rewardedAdUnitId);
    }

    public Task<ViewResult> ShowRewardedAd()
    {
        tcsShowAd = new TaskCompletionSource<ViewResult>();

        if (MaxSdk.IsRewardedAdReady(_adDetails.rewardedAdUnitId))
        {
            Log.Write("Showing");
            MaxSdk.ShowRewardedAd(_adDetails.rewardedAdUnitId);

            return tcsShowAd.Task;
        }

        Log.Write("Ad not ready!");

        tcsShowAd = null;

        return Task.FromResult(new ViewResult {success = false});
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        Log.Write("Rewarded ad loaded");

        // Reset retry attempt
        rewardedRetryAttempt = 0;
    }

    private async void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        var retryDelay = (int) Math.Pow(2, Math.Min(6, rewardedRetryAttempt)) * 1000;

        Log.Write("Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...");
        Log.Write("Rewarded ad failed to load with error code: " + errorInfo.Code);
        await UniTask.Delay(retryDelay);
        LoadRewardedAd();
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
                                                  MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Log.Write("Rewarded ad failed to display with error code: " + errorInfo.Code);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Log.Write("Rewarded ad displayed");
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Log.Write("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Log.Write("Rewarded ad dismissed");

        if (tcsShowAd.Task.IsCompleted == false)
            tcsShowAd?.SetResult(new ViewResult {success = false});

        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        tcsShowAd?.SetResult(new ViewResult
        {
            success = true,
        });
        // Rewarded ad was displayed and user should receive the reward
        Log.Write("Rewarded ad received reward");
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad revenue paid. Use this callback to track user revenue.
        Log.Write("Rewarded ad revenue paid");

        // Ad revenue
        var revenue = adInfo.Revenue;

        // Miscellaneous data
        var countryCode =
            MaxSdk.GetSdkConfiguration()
                  .CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        var networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        var adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        var placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        //TrackAdRevenue(adInfo); TODO: Add this.
    }

    #endregion

    #endregion

    #region Delimetr

    #endregion

    // #region Default implimentation
    //
    // public void InitSdk()
    // {
    //     Log.WriteError("Unknown Ad manager");
    // }
    //
    // public void SetUserId(string userId)
    // {
    //     Log.WriteError("Unknown Ad manager");
    // }
    //
    // public void ShowDebugger()
    // {
    //     Log.WriteError("Unknown Ad manager");
    // }
    //
    // public bool ReadyToWatchRewarded { get; }
    // public event Action OnReadyToWatch;
    //
    // public Task<ViewResult> ShowRewardedAd()
    // {
    //     Log.WriteError("Unknown Ad manager");
    //     var tcs = new TaskCompletionSource<ViewResult>();
    //     tcs.SetResult(new ViewResult{ success = false});
    //     return tcs.Task;
    // }
    //
    // private void InternalInvoke() => OnReadyToWatch?.Invoke();
    //
    // #endregion
}
}