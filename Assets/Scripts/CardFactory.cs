using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

public static class CardFactory
{
    static Dictionary<Guid, Card> cardDict;
    static Dictionary<Guid, Card> CardDict
    {
        get
        {
            if (CardDict == null)
                throw new Exception("You need to called Initalize() first.");

            return cardDict;
        }
        set
        {
            cardDict = value;
        }
    }

    public static void Initialize()
    {
        // add Load assemby code here

        CardDict = new();

        // create card instance that loaded from assembly then put it into dict.
    }

    public static Card GetCard(Guid guid)
    {
        if(cardDict.TryGetValue(guid,out Card card))
        {
            return card;
        }

        throw new Exception("id not found.");
    }

    public static IEnumerable<KeyValuePair<Guid,Card>> GetCards()
    {
        if (CardDict == null)
            throw new Exception("You need to called Initalize() first.");
        
        return CardDict.AsEnumerable();
    }
}
