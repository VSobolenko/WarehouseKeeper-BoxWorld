using System;

namespace WarehouseKeeper.FSMCore
{
internal abstract class State<TInput, TOutput> : BaseState
{
    protected IStateMachine stateMachine;
    protected TInput inputData;

    public override bool IsActiveState => stateMachine?.ActiveState == this;

    public void ActiveState(IStateMachine machine, TInput data)
    {
#if DEVELOPMENT_BUILD
        stateCounter++;
        Extension.Log.WriteInfo($"[{stateCounter}]Active state: {GetType().Name}");
#endif
        inputData = data;
        stateMachine = machine;
        stateMachine?.SwitchState(this);
        OnStateActivated();
    }
    
    public TOutput FinishState()
    {
        try
        {
            return OnStateFinished();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(e);
#elif DEVELOPMENT_BUILD
            Extension.Log.WriteError($"[OnStateFinished] {e.Message}");
#endif
        }
        
        return default;
    }

    public override void ForceFinish()
    {
        FinishState();
    }

    protected virtual void OnStateActivated() { }
    protected abstract TOutput OnStateFinished();
}
}