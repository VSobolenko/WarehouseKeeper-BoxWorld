using System.Collections.Generic;
using Game;
using Game.DynamicData;
using UnityEngine;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.ZenjectDependency
{
public class TickWrapper : ITickable
{
    public readonly List<IUpdatable> items = new();
    
    public void Tick()
    {
        foreach (var item in items)
        {
            item.Update();
        }
    }
}
}