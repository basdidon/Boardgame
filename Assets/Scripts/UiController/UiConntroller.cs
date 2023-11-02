using UnityEngine;
using UnityEngine.UIElements;

namespace Boardgame.Ui
{
    public abstract class UiController : MonoBehaviour
    {
        public VisualElement Root { get; protected set; }

        protected virtual void Awake()
        {
            if (TryGetComponent(out UIDocument uiDoc))
            {
                Root = uiDoc.rootVisualElement;
            }
        }

        public virtual void Display()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public virtual void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }
    }
}