using System;

namespace WarehouseKeeper.FSMCore
{
internal abstract class BaseState : IDisposable
{
    public abstract StateType Type { get; }
    
    public abstract bool IsActiveState { get; }
    
    public void UpdateState()
    {
        try
        {
            OnStateUpdated();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(e);
#elif DEVELOPMENT_BUILD
            Extension.Log.WriteError($"[OnStateFinished] {e.Message}");
#endif
        }
    }
    
    protected virtual void OnStateUpdated() { }

    public abstract void ForceFinish();
    
    public virtual void Dispose() { }
    
#if DEVELOPMENT_BUILD
    protected static int stateCounter;
#endif
}
}