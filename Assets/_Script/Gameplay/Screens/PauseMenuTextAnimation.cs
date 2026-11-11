using System.Collections.Generic;
using Alchemy.Inspector;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

namespace Gimersia.Gameplay.Screen
{
    public class PauseMenuTextAnimation : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public float distance = 30;
        public float duration = 1;
        public float delay = 0.1f;
        public Ease ease = Ease.InOutQuad;
        
        List<MotionHandle> _handles = new();
        
        [Button]
        void AnimateText()
        {
            CleanHandles();
            for (int i = 0; i < text.textInfo.characterCount; i++)
            {
                var m = LMotion.Create(Vector3.zero, Vector3.up * distance, duration)
                    .WithLoops(-1, LoopType.Yoyo)
                    .WithDelay(i * delay)
                    .WithEase(ease)
                    .BindToTMPCharPosition(text, i);
                
                _handles.Add(m);
            }
        }
        void CleanHandles()
        {            
            if (_handles.Count > 0)
            {
                foreach (var handle in _handles)
                    if (handle.IsActive()) handle.TryCancel();
                
                _handles.Clear();
            }
        }
    }
}