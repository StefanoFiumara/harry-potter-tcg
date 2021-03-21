using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Data.Save;
using HarryPotter.Enums;
using HarryPotter.Utils;
using HarryPotter.Views.UI.Cursor;
using HarryPotter.Views.UI.Tooltips;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotter.DeckEditor
{
    public class DeckBuilderView : MonoBehaviour
    {
        [Header("Data")]
        public CardLibrary Library;
        public Player Player;
        
        [Header("Prefabs")]
        public LibraryCardView LibraryCardPrefab;
        public DeckListCardView DeckListCardPrefab;
        
        [Header("ScrollView")]
        public Transform LibraryScrollViewContent;
        public Transform DeckListScrollViewContent;
        
        [Header("Controllers")]
        public TooltipController Tooltip;
        public CursorController Cursor;
        
        [Header("Filters")]
        public TMP_InputField SearchField;

        public Toggle FilterComc;
        public Toggle FilterCharms;
        public Toggle FilterTrans;
        public Toggle FilterPotions;
        public Toggle FilterQuidditch;

        [Header("Child Views")] 
        public DeckSummaryView DeckSummary;

        private List<LibraryCardView> _library;
        private List<DeckListCardView> _deck;

        private Dictionary<LessonType, Toggle> _filterToggles;

        private void Start()
        {
            _library = new List<LibraryCardView>();
            _deck = new List<DeckListCardView>();

            foreach (var card in Library.Cards)
            {
                var cardView = Instantiate(LibraryCardPrefab, LibraryScrollViewContent);
                cardView.InitView(card);
                _library.Add(cardView);
            }
            
            var orderedPlayerDeck = Player.SelectedDeck.Cards
                .SortCards()
                .GroupBy(c => c);
            
            foreach (var groupedCards in orderedPlayerDeck)
            {
                var cardView = Instantiate(DeckListCardPrefab, DeckListScrollViewContent);
                cardView.InitView(groupedCards.Key, groupedCards.Count());
                _deck.Add(cardView);
            }

            _filterToggles = new Dictionary<LessonType, Toggle>
            {
                {LessonType.Creatures, FilterComc},
                {LessonType.Charms, FilterCharms},
                {LessonType.Transfiguration, FilterTrans},
                {LessonType.Potions, FilterPotions},
                {LessonType.Quidditch, FilterQuidditch},
            };
            
            SearchField.onValueChanged.AddListener(OnSearchValueChanged);

            foreach (var toggle in _filterToggles)
            {
                toggle.Value.onValueChanged.AddListener(OnFilterToggleChanged);
            }
        }

        private void OnFilterToggleChanged(bool isToggled)
        {
            var activeTypes = _filterToggles.Where(t => t.Value.isOn).Select(t => t.Key).ToList();

            foreach (var card in _library)
            {
                var lessonType = card.Data.GetDataLessonType();
                if (activeTypes.Contains(lessonType) || lessonType == LessonType.None)
                {
                    card.gameObject.SetActive(true);
                }
                else
                {
                    card.gameObject.SetActive(false);
                }
            }
        }

        private void OnSearchValueChanged(string search)
        {
            // TODO: Optimize with Trie? Contains vs StartsWith?
            foreach (var card in _library)
            {
                if (card.Data.CardName.ToLower().Contains(search.ToLower()))
                {
                    card.gameObject.SetActive(true);
                }
                else
                {
                    card.gameObject.SetActive(false);
                }
            }
        }

        public void OnSaveClicked()
        {
            Global.SaveManager.SaveData();
        }
        
        public void SetStartingCharacter(LibraryCardView card)
        {
            if (card.Data.Type == CardType.Character && card.Data.Tags.HasTag(Tag.Witch | Tag.Wizard))
            {
                Player.SelectedDeck.StartingCharacter = card.Data;
                DeckSummary.SetStartingCharacter(card.Data);
            }
        }

        public void AddCardToDeck(LibraryCardView card)
        {
            var existingCopyCount = Player.SelectedDeck.Cards.Count(c => c == card.Data);

            bool canAddCard = Player.SelectedDeck.Cards.Count < 60
                              && (existingCopyCount < 4 || card.Data.Type == CardType.Lesson);

            if (!canAddCard)
            {
                return;
            }
            
            Player.SelectedDeck.Cards.Add(card.Data);
            DeckSummary.UpdateLessonSummaryText();

            var existingView = _deck.SingleOrDefault(c => c.Data == card.Data);
            if (existingView != null)
            {
                existingView.Count++;
            }
            else
            {
                var newCardView = Instantiate(DeckListCardPrefab, DeckListScrollViewContent);
                newCardView.InitView(card.Data, 1);
                _deck.Add(newCardView);

                // NOTE: We select into a tuple because otherwise unity complains about this orderBy expression for some reason :shrug: 
                // TODO: Find a way to use the ordering defined in SortCards() extension method instead of duplicating the sort logic here.
                var orderedViews = _deck.Select(c => (view: c, c.Data))
                    .OrderBy(c => c.Data.GetDataAttribute<LessonCost>()?.Type)
                    .ThenBy(c => c.Data.Type)
                    .ThenBy(c => c.Data.GetDataAttribute<LessonCost>()?.Amount)
                    .ThenBy(c => c.Data.CardName);
                
                int i = 0;
                foreach (var (view, _) in orderedViews)
                {
                    // Orders the view in the layout group to maintain grouping
                    view.transform.SetSiblingIndex(i++);
                }
            
            }
        }

        public void RemoveCardFromDeck(DeckListCardView card)
        {
            var removed = Player.SelectedDeck.Cards.Remove(card.Data);

            if (removed)
            {
                DeckSummary.UpdateLessonSummaryText();
                
                var view = _deck.Single(c => c.Data == card.Data);

                if (view.Count > 1)
                {
                    view.Count--;
                }
                else
                {
                    _deck.Remove(view);
                    Destroy(view.gameObject);
                    Tooltip.Hide();
                    Cursor.ResetCursor();
                }
            }
        }

        private void OnDestroy()
        {
            _library?.Clear();
            _deck?.Clear();
            SearchField.onValueChanged.RemoveListener(OnSearchValueChanged);
        }
    }
}