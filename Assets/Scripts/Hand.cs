using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

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

    public Hand(IEnumerable<Card> _cards)
    {
        cards = _cards.ToList();
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void Disable()
    {
        foreach(var card in cards)
        {
            card.Dragable = false;
        }
    }

    public void Enable()
    {
        foreach (var card in cards)
        {
            card.Dragable = true;
        }
    }
}
