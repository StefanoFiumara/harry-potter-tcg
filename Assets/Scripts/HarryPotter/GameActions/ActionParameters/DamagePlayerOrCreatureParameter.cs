namespace HarryPotter.GameActions.ActionParameters
{
    public class DamagePlayerOrCreatureParameter : IActionParameter
    {
        public int Amount { get; set; }

        public string Serialize()
        {
            return $"{Amount}";
        }

        public static DamagePlayerOrCreatureParameter FromString(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                return new DamagePlayerOrCreatureParameter();
            }
            
            var damageAmount = int.Parse(param);

            return new DamagePlayerOrCreatureParameter
            {
                Amount = damageAmount,
            };
        }
    }
}