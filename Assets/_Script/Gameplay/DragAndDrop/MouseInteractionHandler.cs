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
        MotionHandle _wobbleMotion;
        float _dragAngle;
        float _dragAngularVelocity;
        float _horizontalMouseVelocity;
        float _horizontalMouseVelocitySmoothing;

        const float WobbleDuration = 2f;
        const float WobbleAngle = 30f;
        const float WobbleCycles = 1.35f;
        const float MaximumDragAngle = 50f;
        const float DragAngleSensitivity = 10f;
        const float MouseVelocitySmoothTime = 0.05f;
        const float DragSpringStrength = 100f;
        const float DragDamping = 6f;

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
                    CancelWobble();
                    draggable.ResetRotation();
                    draggable.BeginDrag();
                    draggable.SetTopCenterPosition(mouseWorldPosition);
                    _previousMousePosition = mouseWorldPosition;
                    _dragAngle = 0f;
                    _dragAngularVelocity = 0f;
                    _horizontalMouseVelocity = 0f;
                    _horizontalMouseVelocitySmoothing = 0f;
                }
            }

            if (_draggedObject == null)
            {
                return;
            }

            if (Mouse.current.leftButton.isPressed)
            {
                var horizontalSpeed = (mouseWorldPosition.x - _previousMousePosition.x)
                    / Mathf.Max(Time.deltaTime, 0.0001f);
                _horizontalMouseVelocity = Mathf.SmoothDamp(
                    _horizontalMouseVelocity,
                    horizontalSpeed,
                    ref _horizontalMouseVelocitySmoothing,
                    MouseVelocitySmoothTime);
                var targetAngle = Mathf.Clamp(
                    -_horizontalMouseVelocity * DragAngleSensitivity,
                    -MaximumDragAngle,
                    MaximumDragAngle);
                var springAcceleration = (targetAngle - _dragAngle) * DragSpringStrength;
                var dampingAcceleration = _dragAngularVelocity * DragDamping;
                _dragAngularVelocity += (springAcceleration - dampingAcceleration) * Time.deltaTime;
                _dragAngle += _dragAngularVelocity * Time.deltaTime;
                _dragAngle = Mathf.Clamp(_dragAngle, -MaximumDragAngle, MaximumDragAngle);
                _draggedObject.transform.rotation = _draggedObject.DefaultRotation
                    * Quaternion.Euler(0f, 0f, _dragAngle);
                _draggedObject.SetTopCenterPosition(mouseWorldPosition);
                _previousMousePosition = mouseWorldPosition;
                return;
            }

            var dropTarget = FindDroppable(mouseWorldPosition);
            _draggedObject.EndDrag(dropTarget);
            _draggedObject.ResetRotation();
            PlayWobble(_draggedObject);
            _draggedObject = null;
        }

        void PlayWobble(Draggable draggable)
        {
            _wobbleMotion = LMotion.Create(0f, 1f, WobbleDuration)
                .WithEase(Ease.OutQuad)
                .Bind(progress =>
                {
                    var wobble = Mathf.Sin(progress * Mathf.PI * 2f * WobbleCycles)
                        * WobbleAngle
                        * (1f - progress);
                    if (progress >= 0.999f)
                    {
                        draggable.ResetRotation();
                        return;
                    }

                    draggable.transform.rotation = draggable.DefaultRotation
                        * Quaternion.Euler(0f, 0f, wobble);
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
