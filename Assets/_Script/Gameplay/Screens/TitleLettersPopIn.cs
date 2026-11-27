using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Gimersia.Gameplay.Screen
{
    public class TitleLettersPopIn : MonoBehaviour
    {
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private float letterDelay = 0.08f;
        [SerializeField] private Ease ease = Ease.OutBack;

        private readonly List<MotionHandle> _handles = new();
        private Transform[] _letters;
        private Vector3[] _targetScales;

        private void Awake()
        {
            int count = transform.childCount;
            _letters = new Transform[count];
            _targetScales = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                _letters[i] = transform.GetChild(i);
                _targetScales[i] = _letters[i].localScale;
            }
        }

        private void OnEnable()
        {
            Play();
        }

        private void OnDisable()
        {
            CancelAnimations();
        }

        private void Play()
        {
            CancelAnimations();

            for (int i = 0; i < _letters.Length; i++)
            {
                Transform letter = _letters[i];
                letter.localScale = Vector3.zero;

                MotionHandle handle = LMotion.Create(Vector3.zero, _targetScales[i], duration)
                    .WithDelay(i * letterDelay)
                    .WithEase(ease)
                    .BindToLocalScale(letter);

                _handles.Add(handle);
            }
        }

        private void CancelAnimations()
        {
            foreach (MotionHandle handle in _handles)
            {
                if (handle.IsActive())
                {
                    handle.TryCancel();
                }
            }

            _handles.Clear();
        }
    }
}
