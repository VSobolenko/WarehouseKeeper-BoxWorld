namespace Game.FSMCore.States
{
public abstract class BaseState : IState
{
    public virtual void UpdateState() { }

    public virtual void Dispose() { }
    
    private protected static int stateCounter;
}
}