using HarryPotter.Game.Player;
using UnityEngine;

namespace HarryPotter.Game.Cards
{
    public abstract class PlayAction : ScriptableObject
    {
        public abstract void Execute(PlayerState owner, PlayerState enemy);
    }
}