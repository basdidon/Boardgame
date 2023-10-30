using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public interface IBoardObject
{
    BoardManager BoardManager => BoardManager.Instance;
    Transform Transform { get; }
    Vector3Int CellPos { get; set; }

    // check logic for BoardOject Can it go target cell
    bool CanMoveTo(Vector3Int cellPos);
}

public class BoardManager:MonoBehaviour
{
    public static BoardManager Instance { get; private set; }
    public Grid MainGrid { get; private set; }

    List<IBoardObject> boardObjects;
    public IReadOnlyList<IBoardObject> BoardObjects => boardObjects;

    public bool AddBoardObject(IBoardObject boardObject)
    {
        // not allow to place object on same position
        if (BoardObjects.Any(_object => _object.CellPos == boardObject.CellPos))
        {
            return false;
        }

        boardObjects.Add(boardObject);
        boardObject.Transform.position = MainGrid.GetCellCenterWorld(boardObject.CellPos);

        return true;
    }

    public void RemoveBoardObject(IBoardObject boardObject)
    {
        boardObjects.Remove(boardObject);
    }

    public IEnumerable<IBoardObject> BoardObjectsOnCell(Vector3Int cellPos)
    {
        return boardObjects.Where(bo => bo.CellPos == cellPos);
    }

    [field: SerializeField] public RectInt MapRect { get; private set; }
    public bool IsOutOfBound(Vector3Int cellPos) => !MapRect.Contains((Vector2Int)cellPos);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        MainGrid = FindFirstObjectByType<Grid>();
        boardObjects = new();
    }

    public bool TryTeleportObject(IBoardObject boardObject, Vector3Int cellPos)
    {
        if (IsOutOfBound(cellPos))    // this position is out of bound
            return false;
        if (!boardObject.CanMoveTo(cellPos))  // this object can't move to target cell
            return false;
        if (cellPos == boardObject.CellPos)  // can't teleport to where it start at
            return false;

        boardObject.CellPos = cellPos;
        return true;
    }
}