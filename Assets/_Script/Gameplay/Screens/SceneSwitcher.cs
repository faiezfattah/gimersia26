using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gimersia.Gameplay.Screen
{
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private Animator transition;
        [SerializeField] private float transitionTime = 1f;

        private bool isSwitching;

        public void LoadNextScene()
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void LoadSceneByName(string sceneName)
        {
            LoadScene(sceneName);
        }

        public void LoadScene(int sceneBuildIndex)
        {
            if (isSwitching) return;

            StartCoroutine(SwitchScene(sceneBuildIndex));
        }

        public void LoadScene(string sceneName)
        {
            if (isSwitching) return;

            StartCoroutine(SwitchScene(sceneName));
        }

        private IEnumerator SwitchScene(int sceneBuildIndex)
        {
            isSwitching = true;

            if (transition != null)
            {
                transition.SetTrigger("Start");
            }

            yield return new WaitForSecondsRealtime(transitionTime);

            SceneManager.LoadScene(sceneBuildIndex);
        }

        private IEnumerator SwitchScene(string sceneName)
        {
            isSwitching = true;

            if (transition != null)
            {
                transition.SetTrigger("Start");
            }

            yield return new WaitForSecondsRealtime(transitionTime);

            SceneManager.LoadScene(sceneName);
        }
    }
}
