using HarryPotter.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HarryPotter.Views.UI
{
    public class MainMenu : MonoBehaviour
    {
        public Player LocalPlayer;
        
        public void ToGameplay()
        {
            // TODO: More robust validation checks 
            if (LocalPlayer.SelectedDeck.Cards.Count != 60 && LocalPlayer.SelectedDeck.Cards.Count != 30)
            {
                Global.OverlayModal.ShowModal("Invalid Deck", "Your deck does not meet all of the deck-building rules! Create a valid deck before jumping into gameplay.");
                return;
            }
            
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.GAMEPLAY);
        }
    }
}