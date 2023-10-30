using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    [SerializeField] List<Card> cards;
    public IReadOnlyList<Card> Cards => cards;

    public int CardLeft => Cards.Count;

    public Deck(List<Card> _cards)
    {
        cards = _cards;
    }

    public void SuffleDeck()
    {

    }

    public bool TryDraw(out Card card)
    {
        card = null;
        if (CardLeft <= 0)
            return false;

        card = Cards[0];
        cards.Remove(card);
        return true;
    }
}