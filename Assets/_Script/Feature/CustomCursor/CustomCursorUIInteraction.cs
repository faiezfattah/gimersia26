using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Gimersia.Feature.CustomCursor
{
    [DefaultExecutionOrder(-100)]
    public class CustomCursorUIInteraction : MonoBehaviour
    {
        public static CustomCursorUIInteraction Instance { get; private set; }

        EventSystem _eventSystem;
        bool _isPressingButton;
        public bool IsPointerOverButton { get; private set; }

        void Awake()
        {
            Instance = this;
            _eventSystem = EventSystem.current;
            if (_eventSystem == null)
            {
                Debug.LogError(
                    $"{nameof(CustomCursorUIInteraction)} requires an active EventSystem.",
                    this);
                enabled = false;
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            if (Mouse.current == null || CustomCursorController.Instance == null)
                return;

            var button = FindButtonUnderPointer();
            IsPointerOverButton = button != null;

            if (Mouse.current.leftButton.wasPressedThisFrame && button != null)
                _isPressingButton = true;
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
                _isPressingButton = false;

            if (_isPressingButton)
            {
                CustomCursorController.Instance.SetMode(CustomCursorMode.ButtonPressed);
                return;
            }

            if (button != null)
                CustomCursorController.Instance.SetMode(CustomCursorMode.Default);
        }

        Button FindButtonUnderPointer()
        {
            var pointerData = new PointerEventData(_eventSystem)
            {
                position = Mouse.current.position.ReadValue()
            };
            var results = new System.Collections.Generic.List<RaycastResult>();
            _eventSystem.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                var button = result.gameObject.GetComponentInParent<Button>();
                if (button != null && button.interactable)
                    return button;
            }

            return null;
        }
    }
}
