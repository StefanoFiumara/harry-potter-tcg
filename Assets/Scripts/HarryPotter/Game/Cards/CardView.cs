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
        
        public void Init(CardData d, CardState cs, PlayerView p)
        {
            Data = d;
            State = cs;
            Owner = p;

            CardFaceRenderer.sprite = Data.Image;
            // TODO: Additional UI Work for showing modified stats go here
            // TODO: If we can get the card templates we can apply all the CardData values here
        }

        public void Highlight(Color c)
        {
            // TODO: Figure out highlight logic
        }

        public void RemoveHighlight()
        {
            // TODO: Figure out highlight logic
        }
    }
}