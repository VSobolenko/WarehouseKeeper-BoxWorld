using System;
using System.Collections.Generic;

namespace WarehouseKeeper.Directors.Game.UserResources.BaseModels
{
[Serializable]
public class StringUniqueContainerResources
{
    public HashSet<string> Values { get; }

    public event Action OnValueUpdate;

    public StringUniqueContainerResources()
    {
        Values = new HashSet<string>();
    }
    
    public StringUniqueContainerResources(IEnumerable<string> collection)
    {
        Values = collection == null ? new HashSet<string>() : new HashSet<string>(collection);
    }

    public void Add(string value)
    {
        var result = Values.Add(value);
        
        if (result)
            OnValueUpdate?.Invoke();
    }

    public void Spend(string value)
    {
        if (Exists(value))
            return;
        
        var result = Values.Add(value);
        if (result)
            OnValueUpdate?.Invoke();
    }

    public bool Exists(string value) => Values.Contains(value);
}
}