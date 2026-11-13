using UnityEngine;
using UnityEngine.InputSystem;

namespace Gimersia.Feature.DragAndDrop
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Draggable : MonoBehaviour
    {
        Camera _worldCamera;
        Vector3 _dragOffset;
        Vector3 _originalPosition;
        bool _isDragging;

        public bool IsDragging => _isDragging;

        void Awake()
        {
            _worldCamera = Camera.main;
            if (_worldCamera == null)
            {
                Debug.LogError($"{nameof(Draggable)} requires a camera tagged MainCamera.", this);
            }
        }

        void Update()
        {
            if (_worldCamera == null || Mouse.current == null)
            {
                return;
            }

            var mousePosition = Mouse.current.position.ReadValue();
            var mouseWorldPosition = GetMouseWorldPosition(mousePosition);

            if (!_isDragging && Mouse.current.leftButton.wasPressedThisFrame)
            {
                var hits = Physics2D.OverlapPointAll(mouseWorldPosition);
                foreach (var hit in hits)
                {
                    if (hit.GetComponentInParent<Draggable>() != this)
                    {
                        continue;
                    }

                    _originalPosition = transform.position;
                    _dragOffset = transform.position - mouseWorldPosition;
                    _isDragging = true;
                    break;
                }
            }

            if (_isDragging)
            {
                if (Mouse.current.leftButton.isPressed)
                {
                    transform.position = mouseWorldPosition + _dragOffset;
                    return;
                }

                _isDragging = false;
                var dropTarget = FindDropTarget(mouseWorldPosition);

                if (dropTarget == null)
                {
                    transform.position = _originalPosition;
                }
                else
                {
                    dropTarget.AcceptDrop(this);
                }
            }
        }

        Vector3 GetMouseWorldPosition(Vector2 screenPosition)
        {
            var depth = Mathf.Abs(_worldCamera.transform.position.z - transform.position.z);
            var worldPosition = _worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
            worldPosition.z = transform.position.z;
            return worldPosition;
        }

        Droppable FindDropTarget(Vector3 position)
        {
            var colliders = Physics2D.OverlapPointAll(position);

            foreach (var collider in colliders)
            {
                var dropTarget = collider.GetComponent<Droppable>();
                if (dropTarget != null)
                {
                    return dropTarget;
                }
            }

            return null;
        }
    }
}
