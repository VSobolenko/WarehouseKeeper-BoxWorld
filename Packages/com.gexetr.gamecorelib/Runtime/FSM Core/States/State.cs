namespace Game.FSMCore.States
{
public abstract class State<TIn, TOut> : DeadState<TIn>
{
    protected internal abstract TOut ReturnProcessedResult();
}
}