using Alchemy.Inspector;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace Gimersia.Gameplay
{
    public class ProgressBar : MonoBehaviour
    {
        public RectTransform Thumb;
        public RectTransform Track;
        [Min(0f)] public float Duration = 0.35f;
        public Ease Ease = Ease.OutCubic;

        [field: SerializeField] public int CurrentStep { get; private set; }

        int _totalSteps;
        MotionHandle _moveHandle;

        void OnDestroy()
        {
            CancelAnimation();
        }

        [Button]
        public void SetTotalStep(int totalSteps)
        {
            _totalSteps = Mathf.Max(0, totalSteps);
            SetLevel(0);
        }

        [Button]
        public void SetLevel(int step)
        {
            AnimateToStep(step);
        }

        [Button]
        public void AddLevel(int level = 1)
        {
            AnimateToStep(CurrentStep + level);
        }

        [Button]
        public void RemoveLevel(int level = 1)
        {
            AnimateToStep(CurrentStep - level);
        }

        void AnimateToStep(int step)
        {
            CurrentStep = ClampLevel(step);

            if (Thumb == null || Track == null) return;

            CancelAnimation();

            float targetX = GetTargetAnchoredX();
            if (Duration <= 0f)
            {
                var anchoredPos = Thumb.anchoredPosition;
                anchoredPos.x = targetX;
                Thumb.anchoredPosition = anchoredPos;
                return;
            }

            _moveHandle = LMotion.Create(Thumb.anchoredPosition.x, targetX, Duration)
                .WithEase(Ease)
                .BindToAnchoredPositionX(Thumb);
        }

        int ClampLevel(int step)
        {
            return _totalSteps > 0 ? Mathf.Clamp(step, 0, _totalSteps) : Mathf.Max(0, step);
        }

        float GetTargetAnchoredX()
        {
            float progress = _totalSteps > 0 ? Mathf.Clamp01((float)CurrentStep / _totalSteps) : 0f;

            float minTrackX = Track.rect.xMin - Thumb.rect.xMin;
            float maxTrackX = Track.rect.xMax - Thumb.rect.xMax;
            float targetTrackLocalX = Mathf.Lerp(minTrackX, maxTrackX, progress);

            Vector3 worldPos = Track.TransformPoint(new Vector3(targetTrackLocalX, 0f, 0f));
            float targetParentLocalX = Thumb.parent != null
                ? Thumb.parent.InverseTransformPoint(worldPos).x
                : worldPos.x;

            float anchorOffsetX = Thumb.localPosition.x - Thumb.anchoredPosition.x;
            return targetParentLocalX - anchorOffsetX;
        }

        void CancelAnimation()
        {
            if (_moveHandle.IsActive())
                _moveHandle.TryCancel();
        }
    }
}
