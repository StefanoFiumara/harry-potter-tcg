using HarryPotter.GameActions;
using HarryPotter.GameActions.PlayerActions;
using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class AISystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            
        }

        public void UseAction()
        {
            if (Container.GameState.CurrentPlayer.ActionsAvailable > 0)
            {
                Debug.Log("*** AI Action ***");
                var drawAction = new DrawCardsAction(Container.GameState.CurrentPlayer, 1, true);
                Container.Perform(drawAction);
            }
            else
            {
                Debug.Log("*** AI Ends Turn ***");
                Container.ChangeTurn();
            }
        }
        
        public void Destroy()
        {
            
        }
    }
}