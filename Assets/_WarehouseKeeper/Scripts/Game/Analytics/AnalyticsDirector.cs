using System;
using System.Collections.Generic;
using Game.Utility;
using GameAnalyticsSDK;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using Zenject;

namespace WarehouseKeeper.Directors.Game.Analytics
{
internal class AnalyticsDirector : IDisposable
{
    private readonly ResourcesDirector _resourcesDirector;
    private readonly SignalBus _signalBus;

    public AnalyticsDirector(ResourcesDirector resourcesDirector, SignalBus signalBus)
    {
        _resourcesDirector = resourcesDirector;
        _signalBus = signalBus;
        InitializeGameAnalytics();
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _signalBus.Subscribe<LevelGoHome>(UserGoHome);
        _signalBus.Subscribe<LevelRestart>(UserRestartLevel);
        _signalBus.Subscribe<LevelStart>(UserStartLevel);
        _signalBus.Subscribe<LevelVictory>(UserCompleteLevel);
        _signalBus.Subscribe<ActivateHint>(UserActivateHint);
        _signalBus.Subscribe<PurchaseAmber>(UserPurchaseProductReal);
        _signalBus.Subscribe<PurchaseProduct>(UserPurchaseProduct);
        _signalBus.Subscribe<ShopEvent>(UserShopEvent);
        _signalBus.Subscribe<ResetProgress>(UserResetProgress);
        _signalBus.Subscribe<UnlockLevelByAmber>(UserBoughtLevel);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<LevelGoHome>(UserGoHome);
        _signalBus.Unsubscribe<LevelRestart>(UserRestartLevel);
        _signalBus.Unsubscribe<LevelStart>(UserStartLevel);
        _signalBus.Unsubscribe<LevelVictory>(UserCompleteLevel);
        _signalBus.Unsubscribe<ActivateHint>(UserActivateHint);
        _signalBus.Unsubscribe<PurchaseAmber>(UserPurchaseProductReal);
        _signalBus.Unsubscribe<PurchaseProduct>(UserPurchaseProduct);
        _signalBus.Unsubscribe<ShopEvent>(UserShopEvent);
        _signalBus.Unsubscribe<ResetProgress>(UserResetProgress);
        _signalBus.Unsubscribe<UnlockLevelByAmber>(UserBoughtLevel);
    }

    private void UserShopEvent(ShopEvent shopEvent)
    {
        GameAnalytics.NewDesignEvent("Shop Event", new Dictionary<string, object>
        {
            {"Message", shopEvent.message},
            {"Time", shopEvent.time},
        });
        Log.Analytics(shopEvent);
    }

    private void UserPurchaseProduct(PurchaseProduct product)
    {
        GameAnalytics.NewBusinessEvent("USD", product.reward.quantity, product.reward.type.ToString(),
                                       product.productId, product.place, new Dictionary<string, object>
                                       {
                                           {"AmberInit", product.amberInitValue},
                                           {"HintInit", product.hintInitValue},
                                           {"Time", product.time},
                                       });
        Log.Analytics(product);
    }

    private void UserPurchaseProductReal(PurchaseAmber product)
    {
        GameAnalytics.NewDesignEvent("AmberPurchaser", new Dictionary<string, object>
        {
            {"ProductId", product.productId},
            {"Result", product.result},
            {"Message", product.message},
            {"Time", product.time},
        });
        Log.Analytics(product);
    }

    private void UserResetProgress(ResetProgress progress)
    {
        GameAnalytics.NewDesignEvent("ResetProgress", progress.countUnlockLevels);
        Log.Analytics(progress);
    }

    private void UserStartLevel(LevelStart level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "levelStart", level.levelId,
                                          new Dictionary<string, object>
                                          {
                                              {"LevelId", level.levelId}
                                          });
        Log.Analytics(level);
    }

    private void UserRestartLevel(LevelRestart level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Undefined, "levelRestart", level.levelId);
        Log.Analytics(level);
    }

    private void UserGoHome(LevelGoHome level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Undefined, "levelGoHome", level.levelId);
        Log.Analytics(level);
    }

    private void UserCompleteLevel(LevelVictory level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "levelComplete", level.levelId,
                                          new Dictionary<string, object>
                                          {
                                              {"LevelId", level.levelId},
                                              {"StarReceived", level.starReceived},
                                              {"ElapsedTime", level.elapsedTime},
                                              {"ActivatedHint", level.countActivatedHint},
                                          });
        Log.Analytics(level);
    }

    private void UserActivateHint(ActivateHint level)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Hint", 1, "GameHint", "GameHint",
                                       new Dictionary<string, object>
                                       {
                                           {"LevelId", level.levelId},
                                       });
        Log.Analytics(level);
    }

    private void UserBoughtLevel(UnlockLevelByAmber level)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Amber", 1, "Level", "BoughtLevel",
                                       new Dictionary<string, object>
                                       {
                                           {"LevelId", level.levelId},
                                       });
        Log.Analytics(level);
    }

    private void InitializeGameAnalytics()
    {
        GameAnalytics.Initialize();
    }
}
}