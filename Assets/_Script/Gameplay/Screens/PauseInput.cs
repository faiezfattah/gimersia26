using Gimersia.Core;
using Gimersia.Feature.Screens;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gimersia.Gameplay.Screen
{
    public class PauseInput : MonoBehaviour
    {
        public ScreenManager ScreenManager;
        public Screens PauseScreen;
        public bool Paused { get; private set; }
        [Inject] InputMap _map;
        void Awake()
        {
            _map.Main.Pause.performed += TogglePause;
        }
        void OnDestroy()
        {
            _map.Main.Pause.performed -= TogglePause;
        }
        void TogglePause(InputAction.CallbackContext context)
        {
            if (Paused)
            {
                ScreenManager.HideCurrent();
                Paused = false;
            }
            else
            {
                ScreenManager.ShowScreen(PauseScreen);
                Paused = true;
            }
        }
    }
}
