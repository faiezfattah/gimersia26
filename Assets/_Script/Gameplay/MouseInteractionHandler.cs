using Gimersia.Core;
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
        [Inject] InputMap _input;
        public LayerMask DroppableLayerMask;
        
        Camera _worldCamera;
        Draggable _currentDraggedObject;
        Draggable _hoveredDraggable;
        Droppable _hoveredDroppable;
        bool _isInputEnabled = true;

        void Awake()
        {
            _worldCamera = Camera.main;
            if (_worldCamera == null)
            {
                Debug.LogError($"{nameof(MouseInteractionHandler)} requires a camera tagged MainCamera.", this);
            }
            // _input.Main.MouseInput
        }

        void Update()
        {
            if (!_isInputEnabled) return;
            
            if (_worldCamera == null || Mouse.current == null) return;

            var mouseWorldPosition = GetMouseWorldPosition();
            UpdateDraggableHover(mouseWorldPosition);
            UpdateDroppableHover(mouseWorldPosition);

            if (_currentDraggedObject == null)
            {
                var draggable = FindDraggable(mouseWorldPosition);
                if (draggable == null) return;

                if (_input.Main.MouseInput.WasPressedThisFrame())
                {
                    _currentDraggedObject = draggable;
                    BeginDrag(draggable, mouseWorldPosition);
                }
            }

            // if still null dont do anything
            if (_currentDraggedObject == null) return;

            if (_input.Main.MouseInput.IsPressed())
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

            if (!draggable.TryGetComponent<Entity>(out var entity))
                throw new System.Exception($"Occupant with valid data doesnt exist on the draggable object {draggable.name}.");

            _interactionCoordinator.OnPickUp(entity);
        }

        void OnDrag(Draggable draggable, Vector3 mouseWorldPosition)
        {
            draggable.UpdateDrag(mouseWorldPosition, Time.deltaTime);
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
