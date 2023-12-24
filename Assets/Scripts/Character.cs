using UnityEngine;
using BasDidon.PathFinder;
using BasDidon.Direction;

public abstract class Character<T> : MonoBehaviour, IBoardObject,IStateActor<T>,ITurnRunner, IDamageable,IPredictMoveable where T:IStateActor<T>
{
    #region IBoardObject
    public Transform Transform => transform;
    [SerializeField] Vector3Int cellPos;
    public Vector3Int CellPos
    {
        get => cellPos;
        set
        {
            cellPos = value;
            transform.position = BoardManager.Instance.MainGrid.GetCellCenterWorld(CellPos);
        }
    }
    #endregion

    [SerializeField] protected int hp = 10;

    public abstract bool CanMoveTo(Vector3Int cellPos);
    public abstract bool TryMove(Vector3Int from, Direction dir,out Vector3Int moveResult);
    public abstract void TakeDamage(int damage);

    protected virtual void Awake()
    {
        CellPos = BoardManager.Instance.MainGrid.WorldToCell(Transform.position);   // self attech to grid
        BoardManager.Instance.AddBoardObject(this);
        TurnManager.Instance.TurnRegister(this);
        TurnManager.Instance.OnTurnChanged += OnTurnChangedHandle;
        // Debug.Log(name);
    }

    protected virtual void Update()
    {
        UpdateState();
    }

    // Events
    protected abstract void OnTurnChangedHandle(ITurnRunner character);

    public void EndTurn()
    {
        TurnManager.Instance.EndTurn(this);
    }

    public IState<T> IdleState { get; protected set; }

    IState<T> state;
    public IState<T> State {
        get => state;
        set
        {
            State?.ExitState();
            state = value ?? IdleState;
            State.StartState();
        }
    }

    public void UpdateState()
    {
        State?.UpdateState();
    }
}
