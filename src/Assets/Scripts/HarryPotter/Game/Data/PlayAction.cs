using HarryPotter.Game.State;
using UnityEngine;

namespace HarryPotter.Game.Data
{
    public abstract class PlayAction : ScriptableObject
    {
        public abstract void Execute(PlayerState owner, PlayerState enemy);
    }
}