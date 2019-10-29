using System.Collections;
using HarryPotter.Data.Cards;

namespace HarryPotter.ActionSystem
{
    public class GameAction
    {
        public bool IsCanceled { get; protected set; }
        
        public GameAction()
        {
            
        }
    }
}