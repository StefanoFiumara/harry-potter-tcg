namespace HarryPotter.GameActions.ActionParameters
{
    public class DiscardActionParameter : IActionParameter
    {
        public int Amount { get; set; }
        public string Serialize()
        {
            return $"{Amount}";
        }
        
        public static DiscardActionParameter FromString(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                return new DiscardActionParameter();
            }
            var parameters = param.Split('|');
            
            var amount = int.Parse(parameters[0]);

            return new DiscardActionParameter
            {
                Amount = amount,
            };
        }
    }
}