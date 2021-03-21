using UnityEngine;
using UnityEngine.SceneManagement;

namespace HarryPotter
{
    public class SceneTransitions : MonoBehaviour
    {
        public void ToMultiplayer()
        {
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.COMING_SOON);
        }

        public void ToDeckEditor()
        {
            Global.Events.Clear();
            SceneManager.LoadScene(Scenes.DECK_BUILDER);
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
