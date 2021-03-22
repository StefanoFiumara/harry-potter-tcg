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
using UnityEngine.SceneManagement;
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
        
        [Header("Filters")]
        public TMP_InputField SearchField;
        
        public Toggle FilterComc;
        public Toggle FilterCharms;
        public Toggle FilterTrans;
        public Toggle FilterPotions;
        public Toggle FilterQuidditch;

        public Toggle FilterLessons;
        public Toggle FilterCreatures;
        public Toggle FilterSpells;
        public Toggle FilterItems;
        public Toggle FilterLocations;
        public Toggle FilterMatches;
        public Toggle FilterAdventures;
        public Toggle FilterCharacters;
        
        [Header("Child Views")] 
        public DeckSummaryView DeckSummary;

        private List<LibraryCardView> _library;
        private List<DeckListCardView> _deck;

        private Dictionary<LessonType, Toggle> _lessonFilterToggles;
        private Dictionary<CardType, Toggle> _typeFilterToggles;

        private bool _isDirty;

        private void Start()
        {
            _library = new List<LibraryCardView>();
            _deck = new List<DeckListCardView>();
            _isDirty = false;

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

            _lessonFilterToggles = new Dictionary<LessonType, Toggle>
            {
                {LessonType.Creatures, FilterComc},
                {LessonType.Charms, FilterCharms},
                {LessonType.Transfiguration, FilterTrans},
                {LessonType.Potions, FilterPotions},
                {LessonType.Quidditch, FilterQuidditch},
            };

            _typeFilterToggles = new Dictionary<CardType, Toggle>
            {
                {CardType.Lesson, FilterLessons},
                {CardType.Creature, FilterCreatures},
                {CardType.Spell, FilterSpells},
                {CardType.Item, FilterItems},
                {CardType.Location, FilterLocations},
                {CardType.Match, FilterMatches},
                {CardType.Adventure, FilterAdventures},
                {CardType.Character, FilterCharacters},
            };
            
            SearchField.onValueChanged.AddListener(OnSearchValueChanged);

            foreach (var toggle in _lessonFilterToggles)
            {
                toggle.Value.onValueChanged.AddListener(OnLessonTypeFilterToggleChanged);
            }

            foreach (var toggle in _typeFilterToggles)
            {
                toggle.Value.onValueChanged.AddListener(OnCardTypeFilterToggleChanged);
            }
        }

        private void OnLessonTypeFilterToggleChanged(bool isToggled)
        {
            var activeTypes = _lessonFilterToggles.Where(t => t.Value.isOn).Select(t => t.Key).ToList();

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

        private void OnCardTypeFilterToggleChanged(bool isToggled)
        {
            var activeTypes = _typeFilterToggles.Where(t => t.Value.isOn).Select(t => t.Key).ToList();
            
            foreach (var card in _library)
            {
                if (activeTypes.Contains(card.Data.Type))
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
            _isDirty = false;
        }

        public void OnExitClicked()
        {
            if (_isDirty)
            {
                Global.OverlayModal.ShowModal(
                    "Save Changes?", 
                    $"Do you want to save the changes you made to the deck {Player.SelectedDeck.DeckName}?", 
                    okCallback: () =>
                    {
                        Global.SaveManager.SaveData();
                        SceneManager.LoadScene(Scenes.MAIN_MENU);
                    },
                    exitCallback: () =>
                    {
                        Global.SaveManager.LoadData();
                        SceneManager.LoadScene(Scenes.MAIN_MENU);
                    });
            }
            else
            {
                Global.SaveManager.LoadData();
                SceneManager.LoadScene(Scenes.MAIN_MENU);
            }
        }
        
        public void SetStartingCharacter(LibraryCardView card)
        {
            if (card.Data == Player.SelectedDeck.StartingCharacter)
            {
                return;
            }
            
            if (card.Data.Type == CardType.Character && card.Data.Tags.HasTag(Tag.Witch | Tag.Wizard))
            {
                Player.SelectedDeck.StartingCharacter = card.Data;
                DeckSummary.SetStartingCharacter(card.Data);
                _isDirty = true;
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
            _isDirty = true;

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
                _isDirty = true;
                
                var view = _deck.Single(c => c.Data == card.Data);
                if (view.Count > 1)
                {
                    view.Count--;
                }
                else
                {
                    _deck.Remove(view);
                    Destroy(view.gameObject);
                    Global.Tooltip.Hide();
                    Global.Cursor.ResetCursor();
                }
            }
        }

        private void OnDestroy()
        {
            _library?.Clear();
            _deck?.Clear();
            SearchField.onValueChanged.RemoveListener(OnSearchValueChanged);

            if (_lessonFilterToggles != null)
            {
                foreach (var toggle in _lessonFilterToggles)
                {
                    toggle.Value.onValueChanged.RemoveListener(OnLessonTypeFilterToggleChanged);
                }
            }


            if (_typeFilterToggles != null)
            {
                foreach (var toggle in _typeFilterToggles)
                {
                    toggle.Value.onValueChanged.RemoveListener(OnCardTypeFilterToggleChanged);
                }
            }
        }
    }
}