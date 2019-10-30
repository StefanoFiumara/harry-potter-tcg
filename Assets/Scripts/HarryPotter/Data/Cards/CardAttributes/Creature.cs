using UnityEngine;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class Creature : CardAttribute
    {
        public int Attack;
        public int Health;

        [HideInInspector] public int DefaultAttack;
        [HideInInspector] public int MaxHealth;

        private void Awake()
        {
            DefaultAttack = Attack;
            MaxHealth = Health;
        }

        public override void ResetAttribute()
        {
            Attack = DefaultAttack;
            Health = MaxHealth;
        }
    }
}