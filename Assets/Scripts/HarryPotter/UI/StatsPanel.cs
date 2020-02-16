using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.Systems;
using TMPro;
using UnityEngine;

namespace HarryPotter.UI
{
    public class StatsPanel : MonoBehaviour
    {
        private const string ICON_CREATURES = @"<sprite name=""icon_creatures"">";
        private const string ICON_CHARMS = @"<sprite name=""icon_charms"">";
        private const string ICON_TRANSFIGURATION = @"<sprite name=""icon_transfiguration"">";
        private const string ICON_POTIONS = @"<sprite name=""icon_potions"">";
        private const string ICON_QUIDDITCH = @"<sprite name=""icon_quidditch"">";

        public TextMeshProUGUI LessonIcons;
        public TextMeshProUGUI LessonText;
        public TextMeshProUGUI ActionsText;
        
        private Player _player;
        
        private readonly Dictionary<LessonType, string> _lessonIconMap = new Dictionary<LessonType, string>
        {
            { LessonType.Creatures, ICON_CREATURES },
            { LessonType.Charms, ICON_CHARMS },
            { LessonType.Transfiguration, ICON_TRANSFIGURATION },
            { LessonType.Potions, ICON_POTIONS },
            { LessonType.Quidditch, ICON_QUIDDITCH },
        };
        private void Awake()
        {
            _player = GetComponentInParent<GameViewSystem>().Game.LocalPlayer;
            
            // TEMP
            LessonIcons.text = string.Join(" ", _lessonIconMap.Select(kvp => kvp.Value));
            LessonText.text = "2";
        }

        private void Update()
        {
            // TODO: Turn this into an event driven component instead of updating the UI every frame.
            UpdateLessonPanel();
            UpdateActionsText();
        }

        private void UpdateActionsText()
        {
            //TODO: Use icon for "Actions" label
            ActionsText.text = $"{_player.ActionsAvailable}";
        }

        private void UpdateLessonPanel()
        {
            var iconsToShow = _lessonIconMap.Where(kvp => _player.LessonTypes.Contains(kvp.Key)).Select(kvp => kvp.Value);
            LessonIcons.text = string.Join(" ", iconsToShow);
            LessonText.text = _player.LessonCount == 0 ? string.Empty : _player.LessonCount.ToString();
        }
    }
}