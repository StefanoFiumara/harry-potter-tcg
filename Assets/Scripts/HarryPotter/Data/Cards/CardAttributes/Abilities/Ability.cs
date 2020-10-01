using HarryPotter.Data.Cards.TargetSelectors;

namespace HarryPotter.Data.Cards.CardAttributes.Abilities
{
    public class Ability : CardAttribute
    {
        //TODO: Chance for abilities to have no target selection?
        public BaseTargetSelector TargetSelector;
        
        public string ActionName;
        public object UserInfo;

        public override void InitAttribute()
        {
            TargetSelector.Owner = Owner;
        }

        public override void ResetAttribute()
        {
            
        }
    }
}