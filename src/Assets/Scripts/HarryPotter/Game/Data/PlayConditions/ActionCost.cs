using HarryPotter.Game.State;
using UnityEngine;

namespace HarryPotter.Game.Data.PlayConditions
{
    public class ActionCost : PlayCondition
    {
        [Range(0, 2)]
        public int Amount;

        public override bool MeetsCondition(PlayerState owner, PlayerState enemy)
        {
            return owner.ActionsAvailable >= Amount;
        }
    }
}