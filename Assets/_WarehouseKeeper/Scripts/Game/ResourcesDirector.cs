using UnityEngine;
using WarehouseKeeper.Directors.Game.Analytics;
using WarehouseKeeper.Directors.Game.Audio;
using WarehouseKeeper.Directors.Game.UserMeta.UserSkins;
using WarehouseKeeper.Directors.UI.Shops;

namespace WarehouseKeeper.Directors
{
// in future use this     [CreateAssetMenu(fileName = nameof(ShopProductsSO), menuName = "Shop/" + nameof(ShopProductsSO))]
internal class ResourcesDirector
{
    public AnimationsSo AnimationsAppearance { get; private set; }
    public BoxSkinSo BoxAppearance { get; private set; }
    public EffectsSo EffectsAppearance { get; private set; }
    public UserSkinSo UserSkinsAppearance { get; private set; }
    public AudioSo AudioData { get; private set; }
    public LocalGameProductsCollectionSo ShopProducts { get; private set; }
    public AnalyticsSo Analytics { get; private set; }

    public ResourcesDirector()
    {
        LoadResources();
    }

    private void LoadResources()
    {
        AnimationsAppearance = Resources.Load<AnimationsSo>("Appearance/AnimationsData");
        BoxAppearance = Resources.Load<BoxSkinSo>("Appearance/BoxSkinsData");
        EffectsAppearance = Resources.Load<EffectsSo>("Appearance/EffectsData");
        UserSkinsAppearance = Resources.Load<UserSkinSo>("Appearance/UserSkinsData");
        AudioData = Resources.Load<AudioSo>("Audio/AudioData");
        ShopProducts = Resources.Load<LocalGameProductsCollectionSo>("Shop/LocalProducts");
        Analytics = Resources.Load<AnalyticsSo>("Analytics/AnalyticsData");
    }
}
}