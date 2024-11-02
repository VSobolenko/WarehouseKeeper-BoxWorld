using Game.Localizations;
using Game.Localizations.Components;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.ZenjectDependency
{
public class LocalizableTMPZenject : LocalizableTMP
{
    [Inject]
    protected override void Initialize(ILocalizationManager localizationManager)
    {
        base.Initialize(localizationManager);
    }
}
}