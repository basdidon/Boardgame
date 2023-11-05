using BasDidon.PathFinder;
using System.Linq;
using UnityEngine;

public sealed class PlayerIdleState : IdleState<Player>
{
    InputProvider InputProvider => Player.Instance.InputProvider;
    CellSelector MoveSelector { get; set; }

    public PlayerIdleState(Player stateActor) : base(stateActor) { }

    public override void OnEnter()
    {
        Debug.Log("Idle");
        var moveableCell = GridPathFinder.PredictMoves(StateActor, 5);

        MoveSelector = new(cell => moveableCell.Any(move => move.ResultCell == cell));
        MoveSelector.OnStart += () =>
        {
            // input logic
            Debug.Log("Enable");
            InputProvider.SelectTarget.Enable();
            InputProvider.SelectTarget.LeftClick.performed += MoveSelector.Choose;
        };
        MoveSelector.OnSuccess += (cell) =>
        {
            var moves = GridPathFinder.PredictMoves(StateActor, 5).FirstOrDefault(move => move.ResultCell == cell);
            Debug.Log(cell);
            StateActor.State = new PlayerMoveState(StateActor, moves.Directions);
        };
        MoveSelector.OnLeave += () =>
        {
            Debug.Log("Disable");
            InputProvider.SelectTarget.Disable();
            InputProvider.SelectTarget.LeftClick.performed -= MoveSelector.Choose;
        };

        MoveSelector.Start();
    }

    public override void OnUpdate() { }
    public override void OnExit() {
        MoveSelector.Cancle();
    }
}
