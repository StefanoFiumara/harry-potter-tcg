using System.Linq;
using HarryPotter.Data;
using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using UnityEngine;
using Utils;

namespace HarryPotter.Systems
{
    public class AISystem : GameSystem
    {
        public void UseAction()
        {
            if (Container.GameState.CurrentPlayer.ActionsAvailable > 0)
            {
                Debug.Log("*** AI Action ***");
                var action = DecideAction(Container.GameState, Container.GameState.CurrentPlayer);
                Container.Perform(action);
            }
            else
            {
                Debug.Log("*** AI Ends Turn ***");
                Container.ChangeTurn();
            }
        }

        private GameAction DecideAction(GameState gameState, Player player)
        {
            if (player.Hand.Any(c => c.Data.Type == CardType.Creature))
            {
                foreach (var card in player.Hand.Where(c => c.Data.Type == CardType.Creature))
                {
                    if (IsCardPlayable(player, card))
                    {
                        return new PlayCardAction(card);
                    }
                }
            }
            
            if (player.Hand.Any(c => c.Data.Type == CardType.Lesson))
            {
                var lesson = player.Hand.First(c => c.Data.Type == CardType.Lesson);
                return new PlayCardAction(lesson);
            }
            
            return new DrawCardsAction(player, 1, true);
        }
        
        private bool IsCardPlayable(Player p, Card c)
        {
            return HasEnoughActions(p, c) && HasEnoughLessons(p, c);
        }

        private bool HasEnoughActions(Player p, Card c)
        {
            var actionCost = c.GetAttribute<ActionCost>();
            return p.ActionsAvailable >= actionCost.Amount;
        }

        private bool HasEnoughLessons(Player p, Card c)
        {
            var lessonCost = c.GetAttribute<LessonCost>();

            if (lessonCost != null)
            {
                return p.LessonCount >= lessonCost.Amount && p.LessonTypes.Contains(lessonCost.Type);
            }

            return true;
        }
    }
}