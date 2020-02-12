using HarryPotter.Systems.Core;
using UnityEngine;

namespace HarryPotter.Systems
{
    public class AISystem : GameSystem, IAwake, IDestroy
    {
        public void Awake()
        {
            
        }

        public void TakeTurn()
        {
            Debug.Log("*** AI Turn ***");
        }
        
        public void Destroy()
        {
            
        }
    }
}