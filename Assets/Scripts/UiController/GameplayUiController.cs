using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Boardgame.Ui
{
    public class GameplayUiController : UiController
    {
        VisualElement DeckPanel { get; set; }
        VisualElement HandPanel { get; set; }
        [field: SerializeField] public VisualTreeAsset CardTreeAsset { get; set; }

        protected override void Awake()
        {
            base.Awake();

            if (Root != null)
            {
                DeckPanel = Root.Q("deck-panel");
                HandPanel = Root.Q("hand-panel");

                DeckPanel.RegisterCallback<ClickEvent>(_ => Player.Instance.DrawCard());
                Player.Instance.OnDrawCard += OnDrawCardHandle;
            }
        }

        public void OnDrawCardHandle(Card newCard)
        {
            var cardVE = CardTreeAsset.Instantiate();
            newCard.BindVisualElement(cardVE);
            cardVE.style.marginLeft = 12;
            cardVE.style.marginRight = 12;
            cardVE.AddManipulator(new CardDragManipulator(cardVE, Root));
            HandPanel.Add(cardVE);
        }
    }
}