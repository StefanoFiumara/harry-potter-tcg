namespace HarryPotter.Data.Cards.CardAttributes
{
    public abstract class RestrictionAttribute : CardAttribute
    {
        public abstract bool MeetsRestriction(Player owner);
    }
}