using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/*
[RequireComponent(typeof(Grid))]
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public Grid MainGrid { get; private set; }
    public Tilemap MainTilemap { get; private set; }
    [field: SerializeField] public RectInt MapSize { get; private set; }

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

        MainGrid = GetComponent<Grid>();


    }
}*/