using Gimersia.Core;
using UnityEngine;

namespace Gimersia.Gameplay.Screen
{
    public class PauseMenu : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public void Show()
        {
            canvasGroup.Show();
        }
        public void Hide()
        {
            canvasGroup.Hide();
        }
    }
}