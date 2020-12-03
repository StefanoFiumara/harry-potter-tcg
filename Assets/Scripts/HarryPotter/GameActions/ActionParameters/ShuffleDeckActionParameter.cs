using System;
using HarryPotter.Enums;

namespace HarryPotter.GameActions.ActionParameters
{
    public class ShuffleDeckActionParameter : IActionParameter
    {
        public Alliance WhichPlayer { get; set; }
        
        public string Serialize()
        {
            return $"{WhichPlayer}";
        }

        public static ShuffleDeckActionParameter FromString(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                return new ShuffleDeckActionParameter();
            }
            
            var parameters = param.Split('|');

            var alliance = (Alliance) Enum.Parse(typeof(Alliance), parameters[0]);

            return new ShuffleDeckActionParameter
            {
                WhichPlayer = alliance
            };
        }
    }
}