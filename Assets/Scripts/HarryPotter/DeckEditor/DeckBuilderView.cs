using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.Utils;
using HarryPotter.Views.UI.Cursor;
using HarryPotter.Views.UI.Tooltips;
using TMPro;
using UnityEngine;

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

        // TODO: Convert TooltipController and CursorController into prefabs so we can edit their properties across all Scenes
        [Header("Controllers")]
        public TooltipController Tooltip;
        public CursorController Cursor;
        public TMP_InputField SearchField;

        private List<LibraryCardView> _library;
        private List<DeckListCardView> _deck;

        private void Awake()
        {
            _library = new List<LibraryCardView>();
            _deck = new List<DeckListCardView>();
            
            foreach (var card in Library.Cards)
            {
                var cardView = Instantiate(LibraryCardPrefab, LibraryScrollViewContent);
                cardView.InitView(card);
                _library.Add(cardView);
            }

            // TODO: Formalize this ordering in some kind of extension, it is also used in UpdateCardLibrary as well as further down in here to re-order cardViews in the deck list. 
            var orderedPlayerDeck = Player.StartingDeck
                .OrderBy(c => c.GetDataAttribute<LessonCost>()?.Type)
                .ThenBy(c => c.Type)
                .ThenBy(c => c.GetDataAttribute<LessonCost>()?.Amount)
                .ThenBy(c => c.CardName)
                .GroupBy(c => c);
            
            foreach (var groupedCards in orderedPlayerDeck)
            {
                var cardView = Instantiate(DeckListCardPrefab, DeckListScrollViewContent);
                cardView.InitView(groupedCards.Key, groupedCards.Count());
                _deck.Add(cardView);
            }
            
            SearchField.onValueChanged.AddListener(OnSearchValueChanged);
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

        public void SetStartingCharacter(LibraryCardView card)
        {
            if (card.Data.Type == CardType.Character && card.Data.Tags.HasTag(Tag.Witch | Tag.Wizard))
            {
                Player.StartingCharacter = card.Data;
            }
        }

        public void AddCardToDeck(LibraryCardView card)
        {
            var existingCopyCount = Player.StartingDeck.Count(c => c == card.Data);

            bool canAddCard = Player.StartingDeck.Count < 60
                              && (existingCopyCount < 4 || card.Data.Type == CardType.Lesson);

            if (!canAddCard)
            {
                return;
            }
            
            Player.StartingDeck.Add(card.Data);

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
                var orderedViews = _deck.Select(c => (view: c, c.Data))
                    .OrderBy(c => c.Data.GetDataAttribute<LessonCost>()?.Type ?? LessonType.None)
                    .ThenBy(c => c.Data.Type)
                    .ThenBy(c => c.Data.GetDataAttribute<LessonCost>()?.Amount ?? 0)
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
            var removed = Player.StartingDeck.Remove(card.Data);

            if (removed)
            {
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
            _library.Clear();
            _deck.Clear();
            SearchField.onValueChanged.RemoveListener(OnSearchValueChanged);
        }
    }
}