using Gimersia.Feature.DragAndDrop;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gimersia.Feature.CustomCursor
{
    public class CustomCursorWorldInteraction : MonoBehaviour
    {
        [SerializeField] Camera _worldCamera;

        bool _isDragging;

        void Awake()
        {
            if (_worldCamera == null)
                _worldCamera = Camera.main;

            if (_worldCamera == null)
            {
                Debug.LogError(
                    $"{nameof(CustomCursorWorldInteraction)} requires a camera tagged MainCamera.",
                    this);
                enabled = false;
            }
        }

        void Update()
        {
            if (Mouse.current == null || CustomCursorController.Instance == null)
                return;
            if (CustomCursorController.Instance.IsButtonPressed)
                return;
            if (CustomCursorUIInteraction.Instance != null &&
                CustomCursorUIInteraction.Instance.IsPointerOverButton)
                return;

            var draggable = FindDraggable(GetMouseWorldPosition());

            if (Mouse.current.leftButton.wasPressedThisFrame && draggable != null)
                _isDragging = true;
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
                _isDragging = false;

            if (_isDragging)
                CustomCursorController.Instance.SetMode(CustomCursorMode.Dragging);
            else if (draggable != null)
                CustomCursorController.Instance.SetMode(CustomCursorMode.Draggable);
            else
                CustomCursorController.Instance.SetMode(CustomCursorMode.Default);
        }

        Vector3 GetMouseWorldPosition()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var depth = Mathf.Abs(_worldCamera.transform.position.z);
            var worldPosition = _worldCamera.ScreenToWorldPoint(
                new Vector3(mousePosition.x, mousePosition.y, depth));
            worldPosition.z = 0f;
            return worldPosition;
        }

        static Draggable FindDraggable(Vector3 position)
        {
            foreach (var collider in Physics2D.OverlapPointAll(position))
            {
                var draggable = collider.GetComponentInParent<Draggable>();
                if (draggable != null)
                    return draggable;
            }

            return null;
        }
    }
}
