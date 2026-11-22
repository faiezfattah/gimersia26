using Gimersia.Gameplay.Screen;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Main";
    [SerializeField] private SceneSwitcher sceneSwitcher;

    public void Play()
    {
        if (sceneSwitcher != null)
        {
            sceneSwitcher.LoadSceneByName(gameSceneName);
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
