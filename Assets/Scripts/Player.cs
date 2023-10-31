using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class Player : Character
{
    public static Player Instance { get; private set; }
    // IBoardObject

    public override bool CanMoveTo(Vector3Int cellPos)
    {
        if (BoardManager.Instance.BoardObjectsOnCell(cellPos).Count() > 0)
            return false;
        return true;
    }

    // Player Deck
    Deck Deck { get; set; }

    //List<Card> Hands { get; set; }
    // Action Point

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
    }

    public void OnTurnChangedHandle(Character character)
    {
        if (character != this)
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
                //put that card to hand
                //Hands.Add(card);
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