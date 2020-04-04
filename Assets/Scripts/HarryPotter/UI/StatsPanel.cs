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
        public TextMeshProUGUI LessonText;
        public TextMeshProUGUI ActionsText;
        
        private Player _player;
        
        private void Awake()
        {
            _player = GetComponentInParent<GameViewSystem>().Game.LocalPlayer;
        }

        private void Update()
        {
            // TODO: Turn this into an event driven component instead of updating the UI every frame.
            UpdateLessonPanel();
            UpdateActionsText();
        }

        private void UpdateActionsText()
        {
            ActionsText.text = _player.ActionsAvailable == 0 
                    ? "-" 
                    : $"{_player.ActionsAvailable}";
        }

        private void UpdateLessonPanel()
        {
            var iconsToShow = TextIcons.LessonIconMap.Where(kvp => _player.LessonTypes.Contains(kvp.Key)).Select(kvp => kvp.Value);
            var lessonCount = _player.LessonCount == 0 ? "-" : _player.LessonCount.ToString();

            LessonText.text = $"{string.Join(" ", iconsToShow)}{lessonCount}";

        }
    }
}