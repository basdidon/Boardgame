using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BasDidon.Direction;

public class MoveState<T> : IState<T>, ISelfExitState where T : IStateActor<T>,IBoardObject
{
    public T StateActor { get; }

    readonly float moveSpeed = 5f;
    protected List<Direction> Directions { get; }
    protected Direction CurrentDirection { get; set; }

    protected Vector3Int StartCell => StateActor.CellPos;
    protected Vector3Int TargetCell => StartCell + CurrentDirection.DirectionVector;


    protected Vector3 StartWorldCenter => BoardManager.Instance.MainGrid.GetCellCenterWorld(StartCell);
    protected Vector3 TargetWorldCenter => BoardManager.Instance.MainGrid.GetCellCenterWorld(TargetCell);

    protected float Duration => 1 / moveSpeed;
    protected float TimeElapsed { get; set; }

    public MoveState(T stateActor, IEnumerable<Direction> directions)
    {
        StateActor = stateActor;
        Directions = directions.ToList();
    }

    public virtual void StartState() 
    {
        if (!Directions.Any())
            return;

        CurrentDirection = Directions[0];
        Directions.RemoveAt(0);

        TimeElapsed = 0;
    }
    public virtual void UpdateState() {
        TimeElapsed += Time.deltaTime;
        StateActor.Transform.position = Vector3.Lerp(StartWorldCenter, TargetWorldCenter, TimeElapsed / Duration);

        if (TimeElapsed >= Duration)
            SetNextState();
    }
    public virtual void ExitState ()
    {
        StateActor.Transform.position = TargetWorldCenter;
        StateActor.CellPos = TargetCell;
    }

    public virtual void SetNextState() {
        if (Directions.Any())
        {
            StateActor.State = new MoveState<T>(StateActor, Directions);
        }
        else
        {
            StateActor.State = null;
        }
    }
}