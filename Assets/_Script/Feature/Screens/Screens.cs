using System.Threading;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using Reflex.Attributes;
using UnityEngine;

namespace Gimersia.Feature.Screens
{
    public class Screens : MonoBehaviour
    {
        [Required] public CanvasGroup CanvasGroup;
        [Inject] protected ScreenManager _screenManager;

        public virtual async UniTask Show(CancellationToken token)
        {
            gameObject.SetActive(true);
            CanvasGroup.blocksRaycasts = true;
            await LMotion
                .Create(0, 1f, 0.3f)
                .BindToAlpha(CanvasGroup)
                .ToUniTask(token);
            CanvasGroup.interactable = true;
        }

        public virtual async UniTask Hide(CancellationToken token)
        {
            CanvasGroup.interactable = false;
            await LMotion
                .Create(1, 0f, 0.3f)
                .BindToAlpha(CanvasGroup)
                .ToUniTask(token);
            CanvasGroup.blocksRaycasts = true;
            gameObject.SetActive(false);
        }

        public void RequestHide()
        {
            _screenManager.HideCurrent();
        }
    }
}
