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
        [SerializeField] Texture2D _defaultCursor;
        [SerializeField] Texture2D _draggableCursor;
        [SerializeField] Texture2D _draggingCursor;
        [SerializeField] Texture2D _buttonPressedCursor;

        [Header("Cursor UI")]
        [SerializeField] Canvas _cursorCanvas;
        [SerializeField] Image _cursorImage;
        [SerializeField] Vector2 _hotspot;
        [SerializeField, Min(0.01f)] float _cursorScale = 1f;

        CustomCursorMode _currentMode = (CustomCursorMode)(-1);
        Sprite _defaultSprite;
        Sprite _draggableSprite;
        Sprite _draggingSprite;
        Sprite _buttonPressedSprite;

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

            _defaultSprite = CreateCursorSprite(_defaultCursor);
            _draggableSprite = CreateCursorSprite(_draggableCursor);
            _draggingSprite = CreateCursorSprite(_draggingCursor);
            _buttonPressedSprite = CreateCursorSprite(_buttonPressedCursor);
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

            DestroyCursorSprite(_defaultSprite);
            DestroyCursorSprite(_draggableSprite);
            DestroyCursorSprite(_draggingSprite);
            DestroyCursorSprite(_buttonPressedSprite);
        }

        public void SetMode(CustomCursorMode mode)
        {
            if (_currentMode == mode)
                return;

            var sprite = mode switch
            {
                CustomCursorMode.Draggable => _draggableSprite,
                CustomCursorMode.Dragging => _draggingSprite,
                CustomCursorMode.ButtonPressed => _buttonPressedSprite,
                _ => _defaultSprite
            };

            if (sprite == null)
            {
                Debug.LogError(
                    $"{nameof(CustomCursorController)} has no cursor texture assigned for {mode}.",
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

        static Sprite CreateCursorSprite(Texture2D texture)
        {
            if (texture == null)
                return null;

            return Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                Vector2.zero,
                100f);
        }

        static void DestroyCursorSprite(Sprite sprite)
        {
            if (sprite != null)
                Destroy(sprite);
        }
    }
}
