using HarryPotter.GameActions;

namespace HarryPotter.Systems.Core
{
    public class Validator
    {
        public bool IsValid { get; private set; }

        public Validator()
        {
            IsValid = true;
        }

        public void Invalidate()
        {
            IsValid = false;
        }
    }

    public static class ValidatorExtensions
    {
        public static bool Validate(this GameAction action)
        {
            var validator = new Validator();
            var eventName = Notification.Validate(action.GetType());
            
            Global.Events.Publish(eventName, validator);

            return validator.IsValid;
        }
    }
}