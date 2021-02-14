using HarryPotter.Data.Cards;

namespace HarryPotter.GameActions.Actions
{
    public class DamageCreatureAction : GameAction
    {
        public Card Source { get; }
        public Card Target { get; }
        public int Amount { get; }
        public bool IsLethal { get; set; }

        public DamageCreatureAction(Card source, Card target, int amount)
        {
            Source = source;
            Target = target;
            Amount = amount;
            IsLethal = false;
            Player = source.Owner;
        }

        // TODO: Ability Loader?

        public override string ToString()
        {
            return $"DamageCreatureAction - {Source.Data.CardName} does {Amount} damage to {Target.Data.CardName}";
        }
    }
}