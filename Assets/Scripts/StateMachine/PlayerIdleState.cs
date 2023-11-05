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
        TurnManager.Instance.OnTurnChanged += OnTurnChangedHandle;

        ActivateSelector();
    }

    public override void OnUpdate() { }
    public override void OnExit() {
        if(MoveSelector.Phase == CellSelector.SelectorPhase.started)
        {
            MoveSelector.Cancle();
        }
    }

    void OnTurnChangedHandle(Character character)
    {
        if (character != StateActor)
            return;

        ActivateSelector();
    }

    void ActivateSelector()
    {
        var moveableCell = GridPathFinder.PredictMoves(StateActor, StateActor.ActionPiont);

        MoveSelector = new(cell => moveableCell.Any(move => move.ResultCell == cell));
        MoveSelector.OnStart += () =>
        {
            // input logic
            InputProvider.SelectTarget.Enable();
            InputProvider.SelectTarget.LeftClick.performed += MoveSelector.Choose;
        };
        MoveSelector.OnSuccess += (cell) =>
        {
            var moves = moveableCell.FirstOrDefault(move => move.ResultCell == cell);
            StateActor.State = new MoveState<Player>(StateActor, moves.Directions);
        };
        MoveSelector.OnLeave += () =>
        {
            InputProvider.SelectTarget.Disable();
            InputProvider.SelectTarget.LeftClick.performed -= MoveSelector.Choose;
        };

        MoveSelector.Start();
    }
}
