using HarryPotter.Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HarryPotter.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void OnClickSinglePlayer()
        {
            SceneManager.LoadScene(Scenes.SINGLE_PLAYER);
        }
        
        public void OnClickMultiplayer()
        {
            SceneManager.LoadScene(Scenes.MULTIPLAYER);
        }

        public void OnClickDeckEditor()
        {
            Debug.Log("Deck Editor Clicked!");
        }

        public void OnClickExit()
        {
            Debug.Log("Exit Clicked!");
            Application.Quit();
        }
    }
}
