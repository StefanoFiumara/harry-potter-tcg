namespace HarryPotter.GameActions.GameFlow
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