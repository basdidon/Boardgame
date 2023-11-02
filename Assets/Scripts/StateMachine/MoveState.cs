public class MoveState<T> : IState, ISelfExitState where T : IStateActor
{
    public T StateActor { get; }

    public MoveState(T stateActor)
    {
        StateActor = stateActor;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }

    public virtual void SetNextState() { }
}