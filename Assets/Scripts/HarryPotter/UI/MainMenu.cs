using UnityEngine;
using UnityEngine.SceneManagement;

namespace HarryPotter.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void OnClickSinglePlayer()
        {
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.SINGLE_PLAYER);
        }
        
        public void OnClickMultiplayer()
        {
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.MULTIPLAYER);
        }

        public void OnClickDeckEditor()
        {
            Global.Events.Clear();
            Debug.Log("Deck Editor Clicked!");
        }

        public void OnClickExit()
        {
            Application.Quit();
        }
    }
}
