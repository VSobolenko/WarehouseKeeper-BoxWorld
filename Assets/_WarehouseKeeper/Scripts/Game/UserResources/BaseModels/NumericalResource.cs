using System;

namespace WarehouseKeeper.Directors.Game.UserResources.BaseModels
{
[Serializable]
public class NumericalResource
{
    private int _value;

    public int Value => _value;

    public event Action OnValueUpdate;

    public NumericalResource(int value)
    {
        _value = value;
    }

    public void Add(int amount)
    {
        _value += amount;
        OnValueUpdate?.Invoke();
    }

    public void Spend(int amount)
    {
        if (CanSpend(amount) == false)
            return;
        
        _value -= amount;
        OnValueUpdate?.Invoke();
    }

    public bool CanSpend(int amount) => _value >= amount;
}
}