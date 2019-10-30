namespace HarryPotter.GameActions
{
    public class ChangeTurnAction : GameAction
    {
        public int NextPlayerIndex { get; }

        public ChangeTurnAction(int nextPlayerIndex)
        {
            NextPlayerIndex = nextPlayerIndex;
        }
    }
}