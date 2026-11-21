using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Gimersia.Gameplay {
    public class CarAnimation : MonoBehaviour {
        public float FromY;
        public Ease EaseIn = Ease.InCubic;
        public float ToY;
        public Ease EaseOut = Ease.OutCubic;
        public float duration = 1f;

        [Button]
        public void AnimateIn()
        {
            LMotion
                .Create(FromY, 0f, duration)
                .WithEase(EaseIn)
                .BindToPositionY(transform);
        }
        [Button]
        public UniTask AnimateOut() {
            return LMotion
                .Create(0f, ToY, duration)
                .WithEase(EaseOut)
                .BindToPositionY(transform)
                .ToUniTask(); ;
        }
    }
}