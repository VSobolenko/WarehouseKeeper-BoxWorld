namespace Game.FSMCore.States
{
public abstract class DeadState<TIn> : BaseState, IActivatedState<TIn>
{
    protected IStateMachine stateMachine;
    protected TIn inputData;

    void IActivatedState<TIn>.ActiveState(IStateMachine machine, TIn data)
    {
        LogDebugInfo();

        inputData = data;
        stateMachine = machine;
        OnStateActivated();
    }
    
    protected virtual void OnStateActivated() { }

    private void LogDebugInfo()
    {
        stateCounter++;
        Log.Info($"[{stateCounter}]Active state: {GetType().Name}");
    }
}
}