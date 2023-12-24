public interface IState<T> where T : IStateActor<T>
{
    public T StateActor { get; }
    public void StartState();
    public void UpdateState();
    public void ExitState();
}