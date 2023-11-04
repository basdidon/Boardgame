using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BasDidon.Direction;

public sealed class PlayerMoveState : MoveState<Player>
{
    readonly float moveSpeed = 5f;
    List<Directions> Directions { get; }
    Directions CurrentDirection { get; set; }

    Vector3Int StartCell => StateActor.CellPos;
    Vector3Int TargetCell => StartCell + DirectionToVector3Int(CurrentDirection);

    Vector3 StartWorldCenter => BoardManager.Instance.MainGrid.GetCellCenterWorld(StartCell);
    Vector3 TargetWorldCenter => BoardManager.Instance.MainGrid.GetCellCenterWorld(TargetCell);

    float duration;
    float timeElapsed;

    public PlayerMoveState(Player stateActor, IEnumerable<Directions> directions) : base(stateActor)
    {
        Directions = directions.ToList();
    }

    public override void OnEnter()
    {
        if (!Directions.Any())
            return;

        CurrentDirection = Directions[0];
        Directions.RemoveAt(0);

        duration = 1 / moveSpeed;
        timeElapsed = 0;
    }

    public override void OnUpdate()
    {
        timeElapsed += Time.deltaTime;
        StateActor.transform.position = Vector3.Lerp(StartWorldCenter, TargetWorldCenter, timeElapsed / duration);

        if (timeElapsed >= duration)
            SetNextState();
    }

    public override void SetNextState()
    {
        if (Directions.Any())
        {
            StateActor.State = new PlayerMoveState(StateActor, Directions);
        }
        else
        {
            StateActor.State = null;
        }
    }

    public override void OnExit()
    {
        StateActor.transform.position = TargetWorldCenter;
        StateActor.CellPos = TargetCell;
    }
}

