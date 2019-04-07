using System.Collections.Generic;
using HarryPotter.Game;
using HarryPotter.Game.State;
using UnityEngine;

namespace HarryPotter.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public PlayerState PlayerState;

        public CardUI CardPrefab;

        public List<CardUI> Cards = new List<CardUI>();
        
        private void Start()
        {
            Cards.Clear();
            
            //Spawn Deck!
            float zPos = 0f;
            foreach (var cardData in PlayerState.StartingDeck)
            {
                var state = new CardState(cardData);

                var card = Instantiate(CardPrefab);
                card.Init(cardData, state);

                card.transform.Translate(0f,0f, zPos);
                zPos += 0.025f;

                PlayerState.Cards.Add(state);

                card.gameObject.name = card.Data.CardName;
                
                Cards.Add(card);
            }
        }
    }
}
