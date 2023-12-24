public interface IStateActor<T> where T : IStateActor<T>
{
    public IState<T> IdleState { get; }  // at default state
    IState<T> State { get; set; }
}