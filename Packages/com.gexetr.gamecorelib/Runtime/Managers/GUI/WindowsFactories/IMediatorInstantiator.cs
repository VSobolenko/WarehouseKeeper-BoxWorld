namespace Game.GUI.Windows.Factories
{
public interface IMediatorInstantiator
{
    TMediator Instantiate<TMediator>(WindowUI windowUI) where TMediator : class, IMediator;
}
}