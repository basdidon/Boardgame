
public class IdleState<T> : IState where T : IStateActor
{
    public T StateActor { get; }

    public IdleState(T stateActor)
    {
        StateActor = stateActor;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}
