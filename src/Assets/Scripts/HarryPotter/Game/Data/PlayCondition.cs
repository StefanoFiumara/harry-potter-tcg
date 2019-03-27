using HarryPotter.Game.State;
using UnityEngine;

namespace HarryPotter.Game.Data
{
    public abstract class PlayCondition : ScriptableObject
    {
        public abstract bool MeetsCondition(PlayerState owner, PlayerState enemy);
    }
}