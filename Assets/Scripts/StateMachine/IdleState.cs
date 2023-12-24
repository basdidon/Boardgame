
public class IdleState<T> : IState<T> where T : IStateActor<T>
{
    public T StateActor { get; }

    public IdleState(T stateActor)
    {
        StateActor = stateActor;
    }

    public virtual void StartState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}
