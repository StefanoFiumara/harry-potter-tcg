using System;
using HarryPotter.Enums;

namespace HarryPotter.GameActions.ActionParameters
{
    public class DamageActionParameter : IActionParameter
    {
        public int DamageAmount { get; set; }
        public Alliance WhichPlayer { get; set; }
        
        public string Serialize()
        {
            return $"{DamageAmount}|{WhichPlayer}";
        }

        public static DamageActionParameter FromString(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                return new DamageActionParameter();
            }
            
            var parameters = param.Split('|');
            var damageAmount = int.Parse(parameters[0]);

            var alliance = parameters.Length > 1 
                    ? (Alliance) Enum.Parse(typeof(Alliance), parameters[1]) 
                    : Alliance.Enemy;

            return new DamageActionParameter
            {
                DamageAmount = damageAmount,
                WhichPlayer = alliance == default ? Alliance.Enemy : alliance
            };
        }
    }
}