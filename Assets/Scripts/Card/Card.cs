using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class Card
{
    public Player Player => Player.Instance;

    public VisualElement CardVE { get; private set; }

    public CardSO CardSO { get; private set; }
    readonly string cardDataURL = "CardsData/";
    public abstract string CardSOName { get; }

    public Card()
    {
        CardSO = Resources.Load(cardDataURL + CardSOName) as CardSO;
    }

    public void BindVisualElement(VisualElement cardVE)
    {
        CardVE = cardVE;
        cardVE.userData = this;

        // update cardVE (set name,cost)
    }

    public abstract void UseCard();
    public string GetClassName()
    {
        return this.GetType().ToString();
    }
}