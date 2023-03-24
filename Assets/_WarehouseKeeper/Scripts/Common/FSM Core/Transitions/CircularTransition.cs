using System;

namespace WarehouseKeeper.FSMCore
{
internal abstract class CircularTransition<TInput, TOutput> : BaseTransition
{
    protected readonly IStateMachine stateMachine;
    private readonly State<TInput, TOutput> _state;

    protected bool IsDecidedTransient => stateMachine.ActiveState.Type == _state.Type;
    
    protected CircularTransition(IStateMachine stateMachine, State<TInput, TOutput> state)
    {
        this.stateMachine = stateMachine;
        _state = state;
    }

    public sealed override void Transit()
    {
        try
        {
            OnTransit();

            if (_state != null)
                _state.FinishState();
            
            _state?.ActiveState(stateMachine, default);
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

    protected virtual void OnTransit() { }
}
}