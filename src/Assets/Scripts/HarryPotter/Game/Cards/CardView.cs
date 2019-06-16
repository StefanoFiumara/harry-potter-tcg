using HarryPotter.Game.Player;
using UnityEngine;

namespace HarryPotter.Game.Cards
{
    public class CardView : MonoBehaviour
    {
        public static readonly Vector3 CARD_SIZE = new Vector3
        {
            x = 5f,
            y = 7f,
            z = 0.25f
        };

        public CardData Data;
        public CardState State;
        public PlayerView Owner;

        public SpriteRenderer CardFaceRenderer;
        
        public void Init(CardData d, CardState cs, PlayerView owner)
        {
            Data = d;
            State = cs;
            
            CardFaceRenderer.sprite = Data.Image;
            // TODO: Additional UI Work for showing modified stats go here
        }
    }
}