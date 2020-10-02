using HarryPotter.Data.Cards;
using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.GameActions.Actions;
using HarryPotter.Systems.Core;
using Utils;

namespace HarryPotter.Systems
{
    public class LessonSystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            Global.Events.Subscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }

        private void OnValidatePlayCard(object sender, object args)
        {
            var action = (PlayCardAction) sender;
            var validator = (Validator) args;

            var lessonCost = action.Card.GetAttribute<LessonCost>();

            if (lessonCost != null && !HasEnoughLessons(action.Card, lessonCost))
            {
                validator.Invalidate("Does not meet lesson requirement");
            }
        }

        public bool HasEnoughLessons(Card card, LessonCost cost)
        {
            var hasEnoughLessons = card.Owner.LessonCount >= cost.Amount;
            var hasLessonType = card.Owner.LessonTypes.Contains(cost.Type);

            return hasEnoughLessons && hasLessonType;
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }
    }
}