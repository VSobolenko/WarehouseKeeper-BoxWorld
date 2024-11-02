using System;

namespace Game.FSMCore.Transitions
{
public abstract class BaseTransition : IDisposable
{
    internal abstract bool Decide();
    
    internal abstract void Transit();
    
    public virtual void Dispose() { }
}
}