using Alchemy.Inspector;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Gimersia.Feature.Blackbar
{
    public class BlackbarManager : MonoBehaviour
    {
        public Image Top;
        public Image Bottom;
        const float _defaultDuration = 1;
        bool _shown = true;
        [Button]
        public void Show(float duration = _defaultDuration)
        {
            if (_shown) return;
            _shown = true;

            LMotion.Create(Top.rectTransform.anchoredPosition.y, -Top.rectTransform.anchoredPosition.y, duration)
                .BindToAnchoredPositionY(Top.rectTransform);

            LMotion.Create(Bottom.rectTransform.anchoredPosition.y, -Bottom.rectTransform.anchoredPosition.y, duration)
                .BindToAnchoredPositionY(Bottom.rectTransform);
        }
        [Button]
        public void Hide(float duration = _defaultDuration)
        {
            if (!_shown) return;
            _shown = false;

            LMotion.Create(Top.rectTransform.anchoredPosition.y, -Top.rectTransform.anchoredPosition.y, duration)
                .BindToAnchoredPositionY(Top.rectTransform);

            LMotion.Create(Bottom.rectTransform.anchoredPosition.y, -Bottom.rectTransform.anchoredPosition.y, duration)
                .BindToAnchoredPositionY(Bottom.rectTransform);
        }
    }
}