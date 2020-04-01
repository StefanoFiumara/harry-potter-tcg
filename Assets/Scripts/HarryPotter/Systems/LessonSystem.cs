using HarryPotter.Data.Cards.CardAttributes;
using HarryPotter.GameActions.PlayerActions;
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

            if (lessonCost != null)
            {
                var hasEnoughLessons = action.Player.LessonCount >= lessonCost.Amount;
                var hasLessonType = action.Player.LessonTypes.Contains(lessonCost.Type);
                
                if (!hasEnoughLessons)
                {
                    validator.Invalidate("Not enough lessons");
                }

                if (!hasLessonType)
                {
                    validator.Invalidate("No matching lesson type");
                }
            }
        }

        public void Destroy()
        {
            Global.Events.Unsubscribe(Notification.Validate<PlayCardAction>(), OnValidatePlayCard);
        }
    }
}