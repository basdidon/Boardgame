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
        for(int i = 0; i < cards.Count; i++)
        {
            var rand = Random.Range(0, cards.Count);

            var temp = cards[i];
            cards[i] = cards[rand];
            cards[rand] = temp;
        }
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