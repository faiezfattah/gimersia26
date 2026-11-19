using Gimersia.Feature.Screens;
using UnityEngine.SceneManagement;

namespace Gimersia.Gameplay.Screen
{
    public class PauseScreen : Screens
    {
        public string mainMenuSceneName = "Splashscreen";
        public void MainMenu() {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}