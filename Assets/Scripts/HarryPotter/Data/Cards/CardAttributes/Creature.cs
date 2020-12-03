namespace HarryPotter.Data.Cards.CardAttributes
{
    public class Creature : CardAttribute
    {
        public int Attack;
        public int Health;

        public int DefaultAttack { get; private set; }
        public int MaxHealth { get; private set; }

        public override void InitAttribute()
        {
            DefaultAttack = Attack;
            MaxHealth = Health;
        }

        public override void ResetAttribute()
        {
            Attack = DefaultAttack;
            Health = MaxHealth;
        }

        public override CardAttribute Clone()
        {
            var copy = CreateInstance<Creature>();
            copy.Attack = Attack;
            copy.Health = Health;
            copy.InitAttribute();
            return copy;
        }
    }
}