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
    public AnimationsScriptableObject AnimationsAppearance { get; private set; }
    public BoxSkinScriptableObject BoxAppearance { get; private set; }
    public EffectsScriptableObject EffectsAppearance { get; private set; }
    public UserSkinScriptableObject UserSkinsAppearance { get; private set; }
    public AudioScriptableObject AudioData { get; private set; }
    public LocalGameProductsCollection ShopProducts { get; private set; }
    public AnalyticsScriptableObject Analytics { get; private set; }

    public ResourcesDirector()
    {
        LoadResources();
    }

    private void LoadResources()
    {
        AnimationsAppearance = Resources.Load<AnimationsScriptableObject>("Appearance/AnimationsData");
        BoxAppearance = Resources.Load<BoxSkinScriptableObject>("Appearance/BoxSkinsData");
        EffectsAppearance = Resources.Load<EffectsScriptableObject>("Appearance/EffectsData");
        UserSkinsAppearance = Resources.Load<UserSkinScriptableObject>("Appearance/UserSkinsData");
        AudioData = Resources.Load<AudioScriptableObject>("Audio/AudioData");
        ShopProducts = Resources.Load<LocalGameProductsCollection>("Shop/LocalProducts");
        Analytics = Resources.Load<AnalyticsScriptableObject>("Analytics/AnalyticsData");
    }
}
}