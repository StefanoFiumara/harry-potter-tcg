using HarryPotter.Enums;

namespace HarryPotter.Data.Cards.CardAttributes
{
    public class PlayRestriction : CardAttribute
    {
        public Alliance WhichPlayer;
        public CardType RestrictedCardTypes;

        // TODO: Other restriction types
        public override void InitAttribute()
        {

        }

        public override void ResetAttribute()
        {

        }

        public override CardAttribute Clone()
        {
            var copy = CreateInstance<PlayRestriction>();
            copy.RestrictedCardTypes = RestrictedCardTypes;
            copy.WhichPlayer = WhichPlayer;
            copy.InitAttribute();
            return copy;
        }
    }
}
