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
            foreach (var cardData in PlayerState.StartingDeck)
            {
                var state = new CardState(cardData);

                var card = Instantiate(CardPrefab);
                card.Init(cardData, state);
                
                card.gameObject.name = card.Data.CardName;
                
                Cards.Add(card);
            }
        }

        
        public void Update()
        {
            //TODO: Update PlayerUI based on PlayerState
        }
    }
}
