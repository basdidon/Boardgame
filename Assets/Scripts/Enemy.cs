using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IBoardObject, IDamageable
{
    public Transform Transform => transform;
    [field: SerializeField] public Vector3Int CellPos { get; set; }
    public bool CanMoveTo(Vector3Int cellPos) => false; // can't move

    [SerializeField] int hp = 10;

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0) {
            BoardManager.Instance.RemoveBoardObject(this);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CellPos = BoardManager.Instance.MainGrid.WorldToCell(Transform.position);   // self attech to grid
        BoardManager.Instance.AddBoardObject(this);
    }
}
