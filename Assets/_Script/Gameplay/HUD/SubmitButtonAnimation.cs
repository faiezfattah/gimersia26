using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gimersia.Gameplay
{
    [RequireComponent(typeof(Button))]
    public class SubmitButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image Image;
        public Sprite[] AvailableSprites;
        public Sprite UnavailableSprite;
        [Min(0.01f)] public float SpriteFrameDuration = 0.2f;
        [Min(0f)] public float HoverDuration = 0.15f;
        [Range(0.1f, 1f)] public float HoverScale = 0.9f;
        [Min(0f)] public float ClickDuration = 0.45f;
        [Min(1f)] public float ClickScale = 1.15f;

        Button _button;
        Image _image;
        Vector3 _baseScale;
        MotionHandle _scaleMotion;
        float _spriteTimer;
        int _spriteIndex;
        bool _isAvailable;
        bool _isHovered;
        bool _isClickAnimating;

        void Awake()
        {
            _button = GetComponent<Button>();
            _image = Image != null ? Image : _button.image;
            _baseScale = transform.localScale;
            _button.transition = Selectable.Transition.None;
            _button.interactable = _isAvailable;

            if (_image == null)
                Debug.LogError($"{nameof(SubmitButtonAnimation)} requires an Image.", this);

            if (AvailableSprites == null || AvailableSprites.Length != 3)
                Debug.LogError($"{nameof(SubmitButtonAnimation)} requires exactly three available sprites.", this);
            if (UnavailableSprite == null)
                Debug.LogError($"{nameof(SubmitButtonAnimation)} requires an unavailable sprite.", this);
            if (_image != null)
            {
                if (_isAvailable && AvailableSprites != null && AvailableSprites.Length == 3)
                    _image.sprite = AvailableSprites[0];
                else
                    _image.sprite = UnavailableSprite;
            }
        }

        void Update()
        {
            if (!_isAvailable || _image == null || AvailableSprites == null || AvailableSprites.Length != 3)
                return;

            _spriteTimer += Time.unscaledDeltaTime;
            float frameDuration = Mathf.Max(0.01f, SpriteFrameDuration);
            while (_spriteTimer >= frameDuration)
            {
                _spriteTimer -= frameDuration;
                _spriteIndex = (_spriteIndex + 1) % AvailableSprites.Length;
                _image.sprite = AvailableSprites[_spriteIndex];
            }
        }

        void OnDisable()
        {
            CancelScaleMotion();
            _isHovered = false;
            _isClickAnimating = false;
            transform.localScale = _baseScale;
        }

        public void SetAvailable()
        {
            _isAvailable = true;
            _button.interactable = true;
            _spriteIndex = 0;
            _spriteTimer = 0f;
            if (_image != null && AvailableSprites != null && AvailableSprites.Length == 3)
                _image.sprite = AvailableSprites[_spriteIndex];
        }

        public void SetUnavailable()
        {
            _isAvailable = false;
            _button.interactable = false;
            _spriteTimer = 0f;
            if (_image != null)
                _image.sprite = UnavailableSprite;
            _isHovered = false;
            _isClickAnimating = false;
            AnimateScale(_baseScale, Ease.OutQuad, HoverDuration);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isAvailable) return;

            _isHovered = true;
            if (!_isClickAnimating)
                AnimateScale(_baseScale * HoverScale, Ease.OutQuad, HoverDuration);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            if (!_isClickAnimating)
                AnimateScale(_baseScale, Ease.OutQuad, HoverDuration);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isAvailable) return;

            CancelScaleMotion();
            _isClickAnimating = true;
            _scaleMotion = LMotion.Create(transform.localScale, _baseScale * ClickScale, ClickDuration)
                .WithEase(Ease.OutElastic)
                .WithOnComplete(() =>
                {
                    _isClickAnimating = false;
                    AnimateScale(_isHovered ? _baseScale * HoverScale : _baseScale, Ease.OutQuad, HoverDuration);
                })
                .BindToLocalScale(transform);
        }

        void AnimateScale(Vector3 targetScale, Ease ease, float duration)
        {
            CancelScaleMotion();

            if (duration <= 0f)
            {
                transform.localScale = targetScale;
                return;
            }

            _scaleMotion = LMotion.Create(transform.localScale, targetScale, duration)
                .WithEase(ease)
                .BindToLocalScale(transform);
        }

        void CancelScaleMotion()
        {
            if (_scaleMotion.IsActive())
                _scaleMotion.TryCancel();
        }
    }
}
