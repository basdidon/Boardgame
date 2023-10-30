using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IBoardObject
{
    public Transform Transform => transform;
    [field: SerializeField] public Vector3Int CellPos { get; set; }
    public bool CanMoveTo(Vector3Int cellPos) => false; // can't move

    private void Start()
    {
        CellPos = BoardManager.Instance.MainGrid.WorldToCell(Transform.position);   // self attech to grid
        BoardManager.Instance.AddBoardObject(this);
    }
}