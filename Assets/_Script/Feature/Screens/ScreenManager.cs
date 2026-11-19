using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace Gimersia.Feature.Screens
{
    public class ScreenManager : MonoBehaviour
    {
        public Screens[] Screens;
        CancellationTokenSource _cts;
        Screens _current;
        public void ShowScreen(string screenName)
        {
            foreach (var screen in Screens)
            {
                if (screen.name == screenName)
                {
                    ShowScreen(screen);
                    return;
                }
            }
            
            Debug.LogError($"Screen with name {screenName} not found.", this);
        }
        public void ShowScreen(Screens screen)
        {
            if (_current == screen) return;
    
            var previous = _current;
            _current = screen;
            ResetToken();
    
            if (previous != null) previous.Hide(_cts.Token).Forget();
            screen.Show(_cts.Token).Forget();
        }
    
        public void HideCurrent()
        {
            if (_current == null) return;
            ResetToken();
            _current.Hide(_cts.Token).Forget();
            _current = null;
        }
    
        void ResetToken()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
        }
    
        void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
