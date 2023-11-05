
public interface IStateActor
{
    public IState IdleState { get;}
    public IState State { get; set; }
    public void UpdateState();
}