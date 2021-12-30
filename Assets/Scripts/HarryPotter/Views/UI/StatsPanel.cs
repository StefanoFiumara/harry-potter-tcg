using HarryPotter.Data;
using HarryPotter.Systems;
using TMPro;
using UnityEngine;

namespace HarryPotter.Views.UI
{
    public class StatsPanel : MonoBehaviour
    {
        public TextMeshProUGUI LessonText;
        public TextMeshProUGUI ActionsText;
        
        public TextMeshProUGUI PlayerDeckCount;
        public TextMeshProUGUI EnemyDeckCount;
        
        private Player _player;
        private Player _enemy;
        
        private void Awake()
        {
            _player = GetComponentInParent<GameView>().Match.LocalPlayer;
            _enemy = GetComponentInParent<GameView>().Match.EnemyPlayer;
        }

        private void Update()
        {
            // TODO: Turn this into an event driven component instead of updating the UI every frame.
            UpdateLessonPanel();
            UpdateActionsText();
            UpdateDeckCounts();
        }

        private void UpdateDeckCounts()
        {
            PlayerDeckCount.text = $"{TextIcons.ICON_DECK} {_player.Deck.Count}";
            EnemyDeckCount.text = $"{TextIcons.ICON_DECK} {_enemy.Deck.Count}";
        }

        private void UpdateActionsText()
        {
            ActionsText.text = _player.ActionsAvailable == 0 
                    ? $"{TextIcons.ICON_ACTIONS} -" 
                    : $"{TextIcons.ICON_ACTIONS} {_player.ActionsAvailable}";
        }

        private void UpdateLessonPanel()
        {
            var lessonTypes = TextIcons.FromLessons(_player.LessonTypes);
            var lessonCount = _player.LessonCount == 0 ? "-" : _player.LessonCount.ToString();

            LessonText.text = $"{lessonTypes} {lessonCount}";
        }
    }
}