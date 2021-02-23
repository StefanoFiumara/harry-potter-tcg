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

            var orderedPlayerDeck = Player.StartingDeck.OrderBy(c => c.GetDataAttribute<LessonCost>()?.Type)
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
            // TODO: Optimize with Trie? Contains vs Starts With?
            foreach (var card in _library)
            {
                if (card.Data.CardName.Contains(search))
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
            if (Player.StartingDeck.Count < 60)
            {
                Player.StartingDeck.Add(card.Data);

                var view = _deck.SingleOrDefault(c => c.Data == card.Data);

                if (view != null)
                {
                    view.Count++;
                }
                else
                {
                    // TODO: How do we insert this in the right spot?
                    var cardView = Instantiate(DeckListCardPrefab, DeckListScrollViewContent);
                    cardView.InitView(card.Data, 1);
                    _deck.Add(cardView);
                }
            }
        }

        private void OnDestroy()
        {
            _library.Clear();
            _deck.Clear();
            SearchField.onValueChanged.RemoveListener(OnSearchValueChanged);
        }

        public void RemoveCardFromDeck(DeckListCardView card)
        {
            // TODO: This might use a different kind of view?
            Player.StartingDeck.Remove(card.Data);

            var view = _deck.Single(c => c.Data == card.Data);

            if (view.Count > 1)
            {
                view.Count--;
            }
            else
            {
                Destroy(view.gameObject);
            }
            
            
        }
    }
}