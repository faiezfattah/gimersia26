using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;
using Gimersia.Feature.DragAndDrop;

namespace Gimersia.Gameplay.DragAndDrop
{
    public class MouseInteractionHandler : MonoBehaviour
    {
        Camera _worldCamera;
        Draggable _draggedObject;
        Vector3 _previousMousePosition;
        Quaternion _originalRotation;
        MotionHandle _wobbleMotion;

        const float WobbleDuration = 1f;
        const float WobbleAngle = 20f;
        const float MinimumWobbleCycles = 1f;
        const float MaximumWobbleCycles = 3f;
        const float SpeedForMaximumWobble = 3f;
        const float MaximumDragAngle = 32f;
        const float DragAngleSensitivity = 8f;
        const float DragAngleSmoothness = 18f;

        void Awake()
        {
            _worldCamera = Camera.main;
            if (_worldCamera == null)
            {
                Debug.LogError($"{nameof(MouseInteractionHandler)} requires a camera tagged MainCamera.", this);
            }
        }

        void Update()
        {
            if (_worldCamera == null || Mouse.current == null)
            {
                return;
            }

            var mouseWorldPosition = GetMouseWorldPosition();

            if (_draggedObject == null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                var draggable = FindDraggable(mouseWorldPosition);
                if (draggable != null)
                {
                    _draggedObject = draggable;
                    _originalRotation = draggable.transform.rotation;
                    draggable.BeginDrag();
                    CancelWobble();
                    draggable.SetTopCenterPosition(mouseWorldPosition);
                    _previousMousePosition = mouseWorldPosition;
                }
            }

            if (_draggedObject == null)
            {
                return;
            }

            if (Mouse.current.leftButton.isPressed)
            {
                var horizontalSpeed = (mouseWorldPosition.x - _previousMousePosition.x) / Time.deltaTime;
                var targetAngle = Mathf.Clamp(
                    -horizontalSpeed * DragAngleSensitivity,
                    -MaximumDragAngle,
                    MaximumDragAngle);
                var targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
                _draggedObject.transform.rotation = Quaternion.Lerp(
                    _draggedObject.transform.rotation,
                    targetRotation,
                    1f - Mathf.Exp(-DragAngleSmoothness * Time.deltaTime));
                _draggedObject.SetTopCenterPosition(mouseWorldPosition);
                _previousMousePosition = mouseWorldPosition;
                return;
            }

            var dropTarget = FindDroppable(mouseWorldPosition);
            _draggedObject.EndDrag(dropTarget);
            PlayWobble(_draggedObject, _previousMousePosition, mouseWorldPosition);
            _draggedObject = null;
        }

        void PlayWobble(Draggable draggable, Vector3 previousPosition, Vector3 releasePosition)
        {
            var releaseSpeed = Vector3.Distance(releasePosition, previousPosition) / Time.deltaTime;
            var wobbleCycles = Mathf.Lerp(
                MinimumWobbleCycles,
                MaximumWobbleCycles,
                Mathf.InverseLerp(0f, SpeedForMaximumWobble, releaseSpeed));
            var startRotation = draggable.transform.rotation;

            _wobbleMotion = LMotion.Create(0f, 1f, WobbleDuration)
                .WithEase(Ease.OutQuad)
                .Bind(progress =>
                {
                    var wobble = Mathf.Sin(progress * Mathf.PI * 2f * wobbleCycles)
                        * WobbleAngle
                        * (1f - progress);
                    var wobbleRotation = _originalRotation * Quaternion.Euler(0f, 0f, wobble);
                    draggable.transform.rotation = Quaternion.Lerp(startRotation, wobbleRotation, progress);
                });
        }

        void CancelWobble()
        {
            if (_wobbleMotion.IsActive())
            {
                _wobbleMotion.TryCancel();
            }
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
                {
                    return draggable;
                }
            }

            return null;
        }

        static Droppable FindDroppable(Vector3 position)
        {
            foreach (var collider in Physics2D.OverlapPointAll(position))
            {
                var droppable = collider.GetComponentInParent<Droppable>();
                if (droppable != null)
                {
                    return droppable;
                }
            }

            return null;
        }
    }
}
