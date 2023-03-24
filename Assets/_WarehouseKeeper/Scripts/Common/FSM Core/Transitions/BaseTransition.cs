using System;

namespace WarehouseKeeper.FSMCore
{
internal abstract class BaseTransition : IDisposable
{
    public abstract bool Decide();
    
    public abstract void Transit();
    
    public virtual void Dispose() { }
}
}