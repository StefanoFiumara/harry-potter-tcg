using HarryPotter.Data.Cards;
using HarryPotter.Utils;
using HarryPotter.Views.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HarryPotter.DeckEditor
{
    public class DeckSummaryView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITooltipContent
    {
        public TMP_Text DeckName;
        public Image StartingCharacterRenderer;
        
        private DeckBuilderView _builderView;
        private CardData StartingCharacter => _builderView.Player.StartingCharacter;

        private void Awake()
        {
            _builderView = GetComponentInParent<DeckBuilderView>();
            
            // IDEA: Maybe replace DeckName Text with Input Field so text name can be edited.
            // TODO: Update this textfield with lesson types in deck when cards are added/removed.
            DeckName.text = _builderView.Player.DeckName;

            if (StartingCharacter != null)
            {
                StartingCharacterRenderer.sprite = StartingCharacter.Image;
            }
        }

        // TODO: OnPointerEnter should only trigger for the StartingCharacterRenderer
        public void OnPointerEnter(PointerEventData eventData)
        {
            _builderView.Tooltip.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _builderView.Tooltip.Hide();
        }

        public string GetDescriptionText() => StartingCharacter.GetFormattedTooltipText();

        public string GetActionText(MonoBehaviour context = null) => string.Empty;
    }
}