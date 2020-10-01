using HarryPotter.Data.Cards.TargetSelectors;
using HarryPotter.Enums;

namespace HarryPotter.Data.Cards.CardAttributes.Abilities
{
    public class Ability : CardAttribute
    {
        public BaseTargetSelector TargetSelector;
        
        public string ActionName;
        public object UserInfo;

        public AbilityType Type;

        public override void InitAttribute()
        {
            TargetSelector.Owner = Owner;
        }

        public override void ResetAttribute()
        {
            
        }
    }
}