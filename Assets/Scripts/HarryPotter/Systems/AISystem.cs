using System.Linq;
using HarryPotter.Data;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using UnityEngine;

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
        
        public GameAction DecideAction(GameState gameState, Player player)
        {
            if (player.Hand.Any(c => c.Data.Type == CardType.Lesson))
            {
                var lesson = player.Hand.First(c => c.Data.Type == CardType.Lesson);
                return new PlayCardAction(lesson);
            }
            
            return new DrawCardsAction(player, 1, true);
        }
    }
}