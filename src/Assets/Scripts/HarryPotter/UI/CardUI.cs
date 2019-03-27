using HarryPotter.Game.Data;
using HarryPotter.Game.State;
using UnityEngine;

namespace HarryPotter.UI
{
    public class CardUI : MonoBehaviour
    {
        public CardData Data;
        public CardState State;
        
        public SpriteRenderer CardFaceRenderer;
        
        public void Init(CardData d, CardState cs)
        {
            Data = d;
            State = cs;
            
            CardFaceRenderer.sprite = Data.Image;
            
            //TODO: Figure out initial positioning (maybe a spawn point?)
            transform.position = new Vector3(-18f, -8f, 22f);
            transform.rotation = Quaternion.Euler(0f,0f,-90f);
            
            //TODO: Additional UI Work for showing modified stats go here
        }
    }
}