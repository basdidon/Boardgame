using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, IBoardObject, IDamageable
{
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

    [SerializeField] protected int hp = 10;

    public abstract bool CanMoveTo(Vector3Int cellPos);
    public abstract void TakeDamage(int damage);

    protected virtual void Awake()
    {
        CellPos = BoardManager.Instance.MainGrid.WorldToCell(Transform.position);   // self attech to grid
        BoardManager.Instance.AddBoardObject(this);
        TurnManager.Instance.TurnRegister(this);
        Debug.Log(name);
    }

    public void EndTurn()
    {
        TurnManager.Instance.EndTurn(this);
    }
}
