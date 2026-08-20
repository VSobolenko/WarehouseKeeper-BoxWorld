using System.Collections.Generic;

namespace Game.Shops
{
public interface IShopCatalog
{
    IReadOnlyCollection<GameProduct> Products { get; }
}
}

