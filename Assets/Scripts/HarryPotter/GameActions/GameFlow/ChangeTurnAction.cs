namespace HarryPotter.GameActions.GameFlow
{
    public class ChangeTurnAction : GameAction
    {
        public int NextPlayerIndex { get; }

        public ChangeTurnAction(int nextPlayerIndex)
        {
            NextPlayerIndex = nextPlayerIndex;
        }

        public override string ToString()
        {
            return $"ChangeTurnAction - It is now Player {NextPlayerIndex + 1}'s Turn.";
        }
    }
}