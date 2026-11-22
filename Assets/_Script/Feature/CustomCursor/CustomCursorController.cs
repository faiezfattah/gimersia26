using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Gimersia.Feature.CustomCursor
{
    public enum CustomCursorMode
    {
        Default,
        Draggable,
        Dragging,
        ButtonPressed
    }

    public class CustomCursorController : MonoBehaviour
    {
        public static CustomCursorController Instance { get; private set; }
        public bool IsButtonPressed => _currentMode == CustomCursorMode.ButtonPressed;

        [Header("Cursor Assets")]
        [SerializeField] Sprite _defaultCursor;
        [SerializeField] Sprite _draggableCursor;
        [SerializeField] Sprite _draggingCursor;
        [SerializeField] Sprite _buttonPressedCursor;

        [Header("Cursor UI")]
        [SerializeField] Canvas _cursorCanvas;
        [SerializeField] Image _cursorImage;
        [SerializeField] Vector2 _hotspot;
        [SerializeField, Min(0.01f)] float _cursorScale = 1f;

        CustomCursorMode _currentMode = (CustomCursorMode)(-1);

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (_cursorCanvas == null || _cursorImage == null)
            {
                Debug.LogError(
                    $"{nameof(CustomCursorController)} requires both a cursor Canvas and Image.",
                    this);
                enabled = false;
                return;
            }

            _cursorImage.raycastTarget = false;
        }

        void OnEnable()
        {
            Cursor.visible = false;
            SetMode(CustomCursorMode.Default);
        }

        void Update()
        {
            if (Mouse.current == null)
                return;

            var canvasTransform = _cursorCanvas.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasTransform,
                Mouse.current.position.ReadValue(),
                _cursorCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? null
                    : _cursorCanvas.worldCamera,
                out var localPosition);

            _cursorImage.rectTransform.anchoredPosition = localPosition;
        }

        void OnDisable()
        {
            Cursor.visible = true;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void SetMode(CustomCursorMode mode)
        {
            if (_currentMode == mode)
                return;

            var sprite = mode switch
            {
                CustomCursorMode.Draggable => _draggableCursor,
                CustomCursorMode.Dragging => _draggingCursor,
                CustomCursorMode.ButtonPressed => _buttonPressedCursor,
                _ => _defaultCursor
            };

            if (sprite == null)
            {
                Debug.LogError(
                    $"{nameof(CustomCursorController)} has no cursor sprite assigned for {mode}.",
                    this);
                _currentMode = mode;
                return;
            }

            _cursorImage.sprite = sprite;
            _cursorImage.SetNativeSize();
            _cursorImage.rectTransform.localScale = Vector3.one * _cursorScale;
            _cursorImage.rectTransform.pivot = new Vector2(
                _hotspot.x / sprite.rect.width,
                1f - _hotspot.y / sprite.rect.height);
            _currentMode = mode;
        }
    }
}
