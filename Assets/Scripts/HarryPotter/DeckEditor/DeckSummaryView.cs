using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Enums;
using HarryPotter.Utils;
using HarryPotter.Views.UI;
using HarryPotter.Views.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HarryPotter.DeckEditor
{
    public class DeckSummaryView : MonoBehaviour
    {
        public Player Player;
        
        public TMP_Text DeckName;
        public TMP_Text LessonSummary;

        public Image StartingCharacterRenderer;
        public CardDataTooltipContent TooltipContentContainer;
        
        private void Start()
        {
            // TODO: Replace DeckName TMP_Text with Input Field so deck name can be edited.
            // TODO: Update this textfield with lesson types in deck when cards are added/removed.
            DeckName.text = Player.SelectedDeck.DeckName;

            if (Player.SelectedDeck.StartingCharacter != null)
            {
                SetStartingCharacter(Player.SelectedDeck.StartingCharacter);
            }
            
            UpdateLessonSummaryText();
        }

        public void SetStartingCharacter(CardData startingCharacter)
        {
            StartingCharacterRenderer.sprite = startingCharacter.Image;
            TooltipContentContainer.CardData = startingCharacter;
        }
        
        public void UpdateLessonSummaryText()
        {
            LessonSummary.text = TextIcons.FromLessons(Player.SelectedDeck.Cards.Where(c => c.Type == CardType.Lesson).GetLessonTypes());
        }
    }
}