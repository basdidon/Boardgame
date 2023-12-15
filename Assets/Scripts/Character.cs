using UnityEngine;
using BasDidon.PathFinder;
using BasDidon.Direction;

public abstract class Character : MonoBehaviour, IBoardObject,IStateActor, IDamageable,IPredictMoveable
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
    protected abstract void OnTurnChangedHandle(Character character);


    public void EndTurn()
    {
        TurnManager.Instance.EndTurn(this);
    }

    public IState IdleState { get; protected set; }

    IState state;
    public IState State {
        get => state;
        set
        {
            State?.OnExit();
            state = value ?? IdleState;
            State.OnEnter();
        }
    }

    public void UpdateState()
    {
        State?.OnUpdate();
    }
}
