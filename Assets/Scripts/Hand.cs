using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    [SerializeField] List<Card> cards;
    public IReadOnlyList<Card> Cards => cards;

    public readonly int maxCards = 10;

    public int CardLeft => Cards.Count;

    public Hand() 
    { 
        cards = new(); 
    }

    public Hand(List<Card> _cards)
    {
        cards = _cards;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }
}
