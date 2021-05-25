using System.Linq;
using HarryPotter.Enums;
using HarryPotter.GameActions;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using UnityEngine;

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
            if (Container.GetMatch().CurrentPlayer.ActionsAvailable > 0)
            {
                Debug.Log("*** AI ACTION ***");
                var action = DecideAction();
                Container.Perform(action);
            }
            else
            {
                Debug.Log("*** AI ENDS TURN ***");
                Container.ChangeTurn();
            }
        }

        private GameAction DecideAction()
        {
            var playable = _cardSystem.PlayableCards;

            var playableSpell = playable.FirstOrDefault(c => c.Data.Type == CardType.Spell);

            if (playableSpell != null)
            {
                return new PlayCardAction(playableSpell);
            }
            
            var playableCreature = playable.FirstOrDefault(c => c.Data.Type == CardType.Creature);

            if (playableCreature != null)
            {
                return new PlayCardAction(playableCreature);
            }

            var activatableCharacter = _cardSystem.ActivatableCards.FirstOrDefault(c => c.Data.Type == CardType.Character);
            if (activatableCharacter != null)
            {
                return new ActivateCardAction(activatableCharacter);
            }
            
            var playableLesson = playable.FirstOrDefault(c => c.Data.Type == CardType.Lesson);
            if (playableLesson != null)
            {
                return new PlayCardAction(playableLesson);
            }
            
            return new DrawCardsAction(Container.GetMatch().CurrentPlayer, 1, true);
        }
    }
}