using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.Actions
{
    public class DamageCreatureAction : GameAction
    {
        public Card Source { get; }
        public Card Target { get; }
        public int Amount { get; }
        
        public DamageCreatureAction(Card source, Card target, int amount)
        {
            Source = source;
            Target = target;
            Amount = amount;
        }
        
        // TODO: Ability Loader?
    }
}