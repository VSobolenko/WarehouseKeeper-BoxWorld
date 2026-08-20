using System;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP
{
internal sealed class UnknownItemIAPConfigurationData : Exception
{
    public UnknownItemIAPConfigurationData(string message) : base(message) { }
}

internal sealed class UnknownIdIAPConfigurationData : Exception
{
    public UnknownIdIAPConfigurationData(string message) : base(message) { }
}
}