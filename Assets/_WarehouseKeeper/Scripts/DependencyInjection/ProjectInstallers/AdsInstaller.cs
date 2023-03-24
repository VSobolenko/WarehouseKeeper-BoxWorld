using WarehouseKeeper.Directors.Common.Managers.Ads;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.ProjectInstallers
{
public class AdsInstaller : Installer<AdsInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<AdDetails>().FromInstance(new AdDetails
        {
            sdkKey = "6nhv2UISHVtgZNl9Ml2fwH5v-MxHFBoVkybv1no4mCaTKIMsxmCNBLJiNBnGLyBeTSV9dCWt2u-I3w1r9wQ_kN",
            rewardedAdUnitId = "79c242342de99d2d",
            interstitialAdUnitId = "9c4f76c0b6c71a2e",
        }).WhenInjectedInto<ApplovinAdManager>();
        Container.Bind<IAdsManager>().To<ApplovinAdManager>().AsSingle().NonLazy();
    }
}
}