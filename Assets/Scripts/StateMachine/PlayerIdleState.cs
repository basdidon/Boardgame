using BasDidon.PathFinder;
using System.Linq;

public sealed class PlayerIdleState : IdleState<Player>
{
    InputProvider inputProvider = Player.Instance.GetComponent<InputProvider>();
    CellSelector selector;

    public PlayerIdleState(Player stateActor) : base(stateActor) { }

    public override void OnEnter()
    {
        var moveableCell = GridPathFinder.PredictMoves(StateActor, 5);

        selector = new(cell => moveableCell.Any(move => move.ResultCell == cell));
        selector.OnSuccess += (cell) =>
        {
            var moves = GridPathFinder.PredictMoves(StateActor, 5).First(move => move.ResultCell == cell);
            StateActor.State = new PlayerMoveState(StateActor, moves.Directions);
        };

        // input logic
        inputProvider.SelectTarget.Enable();
        inputProvider.SelectTarget.LeftClick.performed += _ => selector.Choose();

        selector.Start();
    }

    public override void OnUpdate() { }
    public override void OnExit()
    {
        // input logic
        inputProvider.SelectTarget.Disable();
        inputProvider.SelectTarget.LeftClick.performed -= _ => selector.Choose();
        selector.Cancle();
    }
}
