using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class Card
{
    public Player Player => Player.Instance;

    public VisualElement Root { get; private set; }
    public VisualElement CardVE { get; private set; }
    public CardDragManipulator CardDragManipulator { get; private set; }

    public bool dragable = true;
    public bool Dragable
    {
        get => dragable;
        set
        {
            dragable = value;

            CardVE.RemoveManipulator(CardDragManipulator);
            if (Dragable == true)
                CardVE.AddManipulator(CardDragManipulator);
        }
    }

    public CardSO CardSO { get; private set; }
    string CardDataURL => "CardsData/";
    public abstract string CardSOName { get; }

    public Card()
    {
        CardSO = Resources.Load(CardDataURL + CardSOName) as CardSO;
    }

    public void BindVisualElement(VisualElement cardVE,VisualElement root)
    {
        CardVE = cardVE;
        cardVE.userData = this;

        Root = root;
        CardDragManipulator = new CardDragManipulator(cardVE, Root);
        Dragable = true;
        // update cardVE (set name,cost)
        var cardNameTxt = cardVE.Q<Label>("card-name");
        cardNameTxt.text = CardSO.name;
        var cardCostTxt = cardVE.Q<Label>("card-cost");
        cardCostTxt.text = $"{CardSO.Cost}";
        var cardIcon = cardVE.Q<VisualElement>("card-icon");
        cardIcon.style.backgroundImage = CardSO.Sprite.texture;
    }

    public abstract void UseCard();
    public string GetClassName()
    {
        return this.GetType().ToString();
    }
}