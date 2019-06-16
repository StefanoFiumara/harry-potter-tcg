using HarryPotter.Game.Data;
using HarryPotter.Game.State;
using UnityEngine;

namespace HarryPotter.UI
{
    public class CardUI : MonoBehaviour
    {
        public static readonly Vector3 CARD_SIZE = new Vector3
        {
            x = 5f,
            y = 7f,
            z = 0.25f
        };

        public CardData Data;
        public CardState State;
        
        public SpriteRenderer CardFaceRenderer;
        
        public void Init(CardData d, CardState cs)
        {
            Data = d;
            State = cs;
            
            CardFaceRenderer.sprite = Data.Image;
            // TODO: Additional UI Work for showing modified stats go here
        }
    }
}