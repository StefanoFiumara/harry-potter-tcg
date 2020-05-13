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
    public class AISystem : GameSystem, IAwake
    {
        private CardSystem _cardSystem;

        public void Awake()
        {
            _cardSystem = Container.GetSystem<CardSystem>();
        }
        
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
                    if (_cardSystem.IsPlayable(card))
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
    }
}