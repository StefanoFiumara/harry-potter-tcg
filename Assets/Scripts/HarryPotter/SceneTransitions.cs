using UnityEngine;
using UnityEngine.SceneManagement;

namespace HarryPotter.UI
{
    public class SceneTransitions : MonoBehaviour
    {
        public void ToSinglePlayer()
        {
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.SINGLE_PLAYER);
        }
        
        public void ToMultiplayer()
        {
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.COMING_SOON);
        }

        public void ToDeckEditor()
        {
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.COMING_SOON);
        }
        
        public void ToMainMenu()
        {
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.MAIN_MENU);
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
