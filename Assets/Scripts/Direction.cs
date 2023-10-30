using System;
using UnityEngine;

public static class Direction
{
    [Flags]
    public enum Directions
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        Cardinal = 15, // Left,Right,Up,Down 
    };

    public static Vector3Int[] CardinalVector => new Vector3Int[] { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down };

    public static Directions Vector3IntToDirection(Vector3Int directionVector)
    {
        if(directionVector == Vector3Int.left)
        {
            return Directions.Left;
        }else if(directionVector == Vector3Int.right)
        {
            return Directions.Right;
        }else if(directionVector == Vector3Int.up)
        {
            return Directions.Up;
        }else if(directionVector == Vector3Int.down)
        {
            return Directions.Down;
        }
        else
        {
            Debug.LogWarning($"{directionVector} not in meet any criteria");
            return Directions.None;
        }
    }

    public static bool IsCellInDirection(Vector3Int pivot, Vector3Int cell, Directions dirs)
    {
        Vector3Int _dir = cell - pivot;

        if ((dirs & Directions.Left) != 0 && _dir.x < 0 && _dir.y == 0)
        {
            return true;
        }
        else if ((dirs & Directions.Right) != 0 && _dir.x > 0 && _dir.y == 0)
        {
            return true;
        }
        else if ((dirs & Directions.Up) != 0 && _dir.x == 0 && _dir.y > 0)
        {
            return true;
        }
        else if ((dirs & Directions.Down) != 0 && _dir.x == 0 && _dir.y < 0)
        {
            return true;
        }

        return false;
    }
}
