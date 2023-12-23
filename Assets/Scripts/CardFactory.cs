using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

public static class CardFactory
{
    static Dictionary<string, Type> cardDict;
    static Dictionary<string, Type> CardDict
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
        var cardTypes = Assembly.GetAssembly(typeof(Card)).GetTypes().Where(type=>type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Card)));

        CardDict = new();

        // create card instance that loaded from assembly then put it into dict.
        foreach(var cardType in cardTypes)
        {
            Debug.Log($"{cardType.Name} added to dict.");
            // var tempCard = Activator.CreateInstance(cardType) as Card;
            cardDict.Add(cardType.Name, cardType);
        }
    }

    public static Card GetCard(string id)
    {
        if(cardDict.TryGetValue(id,out Type type))
        {
            return Activator.CreateInstance(type) as Card;
        }

        throw new Exception("id not found.");
    }

    public static IEnumerable<string> GetCardNames()
    {
        return CardDict.Keys;
    }

    public static IEnumerable<KeyValuePair<string,Type>> GetCards()
    {
        if (CardDict == null)
            throw new Exception("You need to called Initalize() first.");
        
        return CardDict.AsEnumerable();
    }
}
