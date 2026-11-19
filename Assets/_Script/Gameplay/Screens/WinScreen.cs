using Gimersia.Feature.Screens;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : Screens
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
