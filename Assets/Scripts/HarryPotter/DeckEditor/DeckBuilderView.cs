using System;
using HarryPotter.Data;
using HarryPotter.Views.UI.Cursor;
using HarryPotter.Views.UI.Tooltips;
using TMPro;
using UnityEngine;

namespace HarryPotter.DeckEditor
{
    public class DeckBuilderView : MonoBehaviour
    {
        public CardLibrary Library;
        public DeckBuilderCardView CardViewPrefab;
        public Transform ScrollViewContent;

        // TODO: Convert TooltipController and CursorController into prefabs so we can edit their properties across all Scenes
        public TooltipController Tooltip;
        public CursorController Cursor;

        public TMP_InputField SearchField;

        private void Awake()
        {
            foreach (var card in Library.Cards)
            {
                var cardView = Instantiate(CardViewPrefab, ScrollViewContent);
                cardView.InitView(card);
            }
            
            // TODO: SearchField.onValueChanged
        }
    }
}