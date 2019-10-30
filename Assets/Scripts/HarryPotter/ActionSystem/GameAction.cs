using HarryPotter.Data;
using HarryPotter.Data.Cards;

namespace HarryPotter.ActionSystem
{
    public class GameAction
    {
        public int Id { get; }
        public bool IsCanceled { get; protected set; }
        
        public Phase PreparePhase { get; protected set; }
        public Phase PerformPhase { get; protected set; }

        public int Priority { get; set; }
        public int OrderOfPlay { get; set; }
        
        public Player Player { get; set; }
        
        public GameAction()
        {
            Id = Global.GenerateId(GetType());
            PreparePhase = new Phase(this, OnPrepare);
            PerformPhase = new Phase(this, OnPerform);
        }

        public virtual void Cancel()
        {
            IsCanceled = true;
        }

        protected virtual void OnPrepare(GameState gameState)
        {
            var eventName = Global.PrepareNotification(GetType());
            Global.Events.Publish(eventName, this);
        }

        protected virtual void OnPerform(GameState gameState)
        {
            var eventName = Global.PerformNotification(GetType());
            Global.Events.Publish(eventName, this);
        }
    }
}