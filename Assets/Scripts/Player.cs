using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour, IBoardObject
{
    public static Player Instance { get; private set; }
    // IBoardObject
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

    public bool CanMoveTo(Vector3Int cellPos)
    {
        if (BoardManager.Instance.BoardObjectsOnCell(cellPos).Count() > 0)
            return false;
        return true;
    }

    // Player Deck
    Deck Deck { get; set; }

    // Action Point

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        var cardList = new List<Card>();

        for (int i = 0; i < 10; i++)
        {
            cardList.Add(new FireBallCard());
        }

        Deck = new(cardList);
    }

    private void Start()
    {
        Deck.SuffleDeck();
        DrawCard(5);
        TurnManager.Instance.OnTurnChanged += OnTurnChangedHandle;
        TurnManager.Instance.TurnRegister(this);
        BoardManager.Instance.AddBoardObject(this);
    }

    public void OnTurnChangedHandle(Player player)
    {
        if (player != this)
            return;

        Debug.Log("My Turn!!");
    }

    public void DrawCard(int n = 1)
    {
        if (n < 1)
            return;

        for (int i = 0; i < n; i++)
        {
            if (Deck.TryDraw(out Card card))
            {
                OnDrawCard?.Invoke(card);
            }
            else
            {
                Debug.Log("No Card Left");
            }
        }
    }

    public Action<Card> OnDrawCard;

    public void Move(Vector3Int direction)
    {
        BoardManager.Instance.TryTeleportObject(this,CellPos + direction);
    }
    
}