using Gimersia.Feature.DragAndDrop;
using Gimersia.Feature.Occupancy;
using LitMotion;
using Oneiric.Core.EventBus;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gimersia.Gameplay
{
    public class MouseInteractionHandler : MonoBehaviour
    {
        [Inject] GameplayInteractionCoordinator _interactionCoordinator;
        public LayerMask DroppableLayerMask;

        Camera _worldCamera;
        Draggable _currentDraggedObject;
        Vector3 _prevMousePos;
        float _dragAngle;
        float _dragAngularVelocity;
        float _horizontalMouseVelocity;
        float _horizontalMouseVelocitySmoothing;
        Draggable _hoveredDraggable;
        Droppable _hoveredDroppable;

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
            if (_worldCamera == null || Mouse.current == null) return;

            var mouseWorldPosition = GetMouseWorldPosition();
            UpdateDraggableHover(mouseWorldPosition);
            UpdateDroppableHover(mouseWorldPosition);

            if (_currentDraggedObject == null)
            {
                var draggable = FindDraggable(mouseWorldPosition);
                if (draggable == null) return;

                _prevMousePos = mouseWorldPosition;

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    _currentDraggedObject = draggable;
                    BeginDrag(draggable, mouseWorldPosition);
                }
            }

            // if still null dont do anything
            if (_currentDraggedObject == null) return;

            if (Mouse.current.leftButton.isPressed)
            {
                OnDrag(_currentDraggedObject, mouseWorldPosition);
                return;
            }

            var dropTarget = FindDroppable(mouseWorldPosition);
            OnDrop(dropTarget);
        }

        void BeginDrag(Draggable draggable, Vector3 mouseWorldPosition)
        {
            draggable.ResetRotation();
            draggable.BeginDrag();
            draggable.SetTopCenterPosition(mouseWorldPosition);

            _dragAngle = 0f;
            _dragAngularVelocity = 0f;

            _horizontalMouseVelocity = 0f;
            _horizontalMouseVelocitySmoothing = 0f;

            if (!draggable.TryGetComponent<Entity>(out var entity))
                throw new System.Exception($"Occupant with valid data doesnt exist on the draggable object {draggable.name}.");

            _interactionCoordinator.OnPickUp(entity);
        }

        void OnDrag(Draggable draggable, Vector3 mouseWorldPosition)
        {
            var horizontalSpeed = (mouseWorldPosition.x - _prevMousePos.x)
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
            _currentDraggedObject.transform.rotation = _currentDraggedObject.DefaultRotation * Quaternion.Euler(0f, 0f, _dragAngle);

            _currentDraggedObject.SetTopCenterPosition(mouseWorldPosition);
            _prevMousePos = mouseWorldPosition;

            _interactionCoordinator.OnDrag(draggable, _hoveredDroppable);
        }

        void UpdateDraggableHover(Vector3 mouseWorldPosition)
        {
            var draggable = FindDraggable(mouseWorldPosition);

            if (draggable == _hoveredDraggable)
                return;

            if (_hoveredDraggable != null)
                OnHoverExit(_hoveredDraggable);

            _hoveredDraggable = draggable;

            if (_hoveredDraggable != null)
                OnHoverEnter(_hoveredDraggable);
        }

        void UpdateDroppableHover(Vector3 mouseWorldPosition)
        {
            var droppable = FindDroppable(mouseWorldPosition);

            if (droppable == _hoveredDroppable)
                return;

            if (_hoveredDroppable != null)
                OnDroppableHoverExit(_hoveredDroppable);

            _hoveredDroppable = droppable;

            if (_hoveredDroppable != null)
                OnDroppableHoverEnter(_hoveredDroppable);
        }

        void OnHoverEnter(Draggable draggable)
        {
            _interactionCoordinator.OnHover(draggable);
        }

        void OnHoverExit(Draggable draggable)
        {
            _interactionCoordinator.OnHoverExit(draggable);
        }

        void OnDroppableHoverEnter(Droppable droppable)
        {
            Debug.Log($"Droppable hover entered: {droppable.name}");
        }

        void OnDroppableHoverExit(Droppable droppable)
        {
            Debug.Log($"Droppable hover exited: {droppable.name}");
        }

        void OnDrop(Droppable dropTarget)
        {
            var draggedObject = _currentDraggedObject;
            var accepted = TryAcceptDrop(draggedObject, dropTarget);

            draggedObject.EndDrag(accepted ? dropTarget : null);
            draggedObject.ResetRotation();
            _currentDraggedObject = null;

            _interactionCoordinator.OnDrop();

            EventBus.Raise(new EntityDropEvent(
                draggedObject.GetComponent<Entity>(),
                dropTarget?.GetComponent<Occupiable>(),
                dropTarget != null ? dropTarget.transform.position : Vector3.zero,
                accepted));
        }

        bool TryAcceptDrop(Draggable draggedObject, Droppable dropTarget)
        {
            if (draggedObject == null || dropTarget == null)
                return false;

            if (!draggedObject.TryGetComponent(out Entity entity))
                return false;

            if (!dropTarget.TryGetComponent(out Occupiable destination))
                return false;

            if (!_interactionCoordinator.TryDropEntity(entity, destination))
                return false;

            draggedObject.transform.parent = dropTarget.transform;
            return true;
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

        Droppable FindDroppable(Vector3 position)
        {
            foreach (var collider in Physics2D.OverlapPointAll(position, DroppableLayerMask))
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
    
    public readonly struct EntityDropEvent : IEvent
    {
        public readonly Entity Entity;
        public readonly Occupiable Destination;
        public readonly Vector3 WorldPosition;
        public readonly bool Accepted;

        public EntityDropEvent(Entity entity, Occupiable destination, Vector3 worldPosition, bool accepted)
        {
            Entity = entity;
            Destination = destination;
            WorldPosition = worldPosition;
            Accepted = accepted;
        }
    }
}
