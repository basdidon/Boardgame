using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using BasDidon.Direction;

public class Player : Character<Player>
{
    public static Player Instance { get; private set; }
    public InputProvider InputProvider { get; private set; }

    #region IBoardObject Implements
    public override bool CanMoveTo(Vector3Int cellPos)
    {
        if (BoardManager.Instance.BoardObjectsOnCell(cellPos).Any())
            return false;
        return true;
    }

    public override bool TryMove(Vector3Int from, Direction dir, out Vector3Int moveResult)
    {
        moveResult = CellPos;
        var targetCell = from + dir.DirectionVector;
        if (!CanMoveTo(targetCell))
            return false;

        /// some scenario we move character it won't stop at target cell
        /// for example, if targetCell are iceFloor tile it keep moving character at start direction and stop on another cell
        /// but i will handle it later

        moveResult = targetCell;
        return true;
    }
    #endregion

    // Player Deck
    Deck Deck { get; set; }
    Hand Hand { get; set; }

    // Action Point
    int MaxActionPoint => 3;
    public int ActionPiont { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        InputProvider = GetComponent<InputProvider>();

        var cardList = new List<Card>();

        for (int i = 0; i < 10; i++)
        {
            cardList.Add(CardFactory.GetCard("FireBallCard")); //new FireBallCard()); 
            cardList.Add(CardFactory.GetCard("Teleport")); // new Teleport()); ;
        }

        Deck = new(cardList);
        Hand = new();

        IdleState = new PlayerIdleState(this);
        State = IdleState;
        
    }

    private void Start()
    {
        Deck.SuffleDeck();
        DrawCard(5);
    }

    protected override void OnTurnChangedHandle(ITurnRunner character)
    {
        if (ReferenceEquals(character,this))
            return;

        Debug.Log("My Turn!!");
        ActionPiont = MaxActionPoint;
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
                //put that card to hand
                Hand.AddCard(card);
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

    public override void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            BoardManager.Instance.RemoveBoardObject(this);
            Destroy(gameObject);
        }
    }
}