
public interface IStateActor
{
    public IState IdleState { get;}
    public IState State { get;}
    public void UpdateState();
}