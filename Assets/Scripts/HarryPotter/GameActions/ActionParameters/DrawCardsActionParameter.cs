using System;
using HarryPotter.Enums;

namespace HarryPotter.GameActions.ActionParameters
{
    public class DrawCardsActionParameter : IActionParameter
    {
        public int Amount { get; set; }
        public Alliance WhichPlayer { get; set; }
        
        public string Serialize()
        {
            return $"{Amount}|{WhichPlayer}";
        }

        public static DrawCardsActionParameter FromString(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                return new DrawCardsActionParameter();
            }
            
            var parameters = param.Split('|');
            var amount = int.Parse(parameters[0]);

            var alliance = parameters.Length > 1 
                ? (Alliance) Enum.Parse(typeof(Alliance), parameters[1]) 
                : Alliance.Ally;

            return new DrawCardsActionParameter
            {
                Amount = amount,
                WhichPlayer = alliance
            };
        }
    }
}