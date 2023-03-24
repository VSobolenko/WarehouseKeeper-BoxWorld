using System;
using System.Collections.Generic;
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
        GameAnalytics.Initialize();
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
        _signalBus.Unsubscribe<ResetProgress>(UserResetProgress);
        _signalBus.Unsubscribe<UnlockLevelByAmber>(UserBoughtLevel);
    }

    private void UserPurchaseProduct(PurchaseProduct product)
    {
        GameAnalytics.NewBusinessEvent("USD", product.reward.quantity, product.reward.type.ToString(),
                                       product.productId, product.place, new Dictionary<string, object>
                                       {
                                           {"AmberInit", product.amberInitValue},
                                           {"HintInit", product.hintInitValue},
                                       });
    }
    
    private void UserPurchaseProductReal(PurchaseAmber product)
    {
        GameAnalytics.NewDesignEvent("AmberPurchaser", new Dictionary<string, object>
        {
            {"Result", product.result},
        });
    }
    
    private void UserResetProgress(ResetProgress progress)
    {
        GameAnalytics.NewDesignEvent("ResetProgress", progress.countUnlockLevels);
    }

    private void UserStartLevel(LevelStart level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "levelStart", level.levelId);
    }
    
    private void UserRestartLevel(LevelRestart level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Undefined, "levelRestart", level.levelId);
    }
    
    private void UserGoHome(LevelGoHome level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Undefined, "levelGoHome", level.levelId);
    }

    private void UserCompleteLevel(LevelVictory level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "levelComplete", level.levelId, new Dictionary<string, object>
        {
            {"LevelId", level.levelId},
            {"StarReceived", level.starReceived},
            {"ElapsedTime", level.elapsedTime},
            {"ActivatedHint", level.countActivatedHint},
        });
    }

    private void UserActivateHint(ActivateHint level)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Hint", 1, "GameHint", "GameHint",
                                       new Dictionary<string, object>
                                       {
                                           {"LevelId", level.levelId},
                                       });
    }
    
    private void UserBoughtLevel(UnlockLevelByAmber level)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Amber", 1, "Level", "BoughtLevel",
                                       new Dictionary<string, object>
                                       {
                                           {"LevelId", level.levelId},
                                       });
    }
}
}