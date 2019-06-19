using HarryPotter.Game.Player;
using UnityEngine;

namespace HarryPotter.Game.Cards
{
    public abstract class PlayCondition : ScriptableObject
    {
        public abstract bool MeetsCondition(PlayerState owner, PlayerState enemy);

        public abstract void Satisfy(PlayerState owner, PlayerState enemy);
    }
}