using System.Collections.Generic;
using Gimersia.Feature.DragAndDrop;
using Gimersia.Feature.Occupancy;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Pool;

namespace Gimersia.Gameplay
{
    [SourceGeneratorInjectable]
    public partial class GameplayInteractionCoordinator
    {
        [Inject] HUDManager _hud;
        [Inject] DialogueSystem _dialogueSystem;
        [Inject] OccupancyManager _occupancyManager;
        [Inject] LevelSystem _levelSystem;

        readonly HashSet<Transform> _activeDialogueTargets = new();

        public void OnPickUp(Entity entity)
        {
            _occupancyManager.PickUpOccupant(entity.occupant);
            _occupancyManager.ShowDroppableTiles();
            UpdateViolationDialogues();
        }

        public void CancelPickUp(Entity entity)
        {
            if (entity == null)
                return;

            _occupancyManager.CancelPickUp(entity.occupant);
        }

        public void OnDrop()
        {
            _occupancyManager.HideDroppableTiles();
            _occupancyManager.HideAllTileMarkers();
            UpdateViolationDialogues();
        }
        public void OnDrag(Draggable draggable, Droppable droppable)
        {
            if (!draggable.TryGetComponent(out Entity entity))
            {
                Debug.LogError($"Draggable {draggable.name} does not have an Entity component.");
                return;
            }

            _occupancyManager.HideAllTileMarkers();

            if (droppable == null ||
                !droppable.TryGetComponent(out Occupiable destination))
            {
                return;
            }

            var cell = destination.Grid.tilemap.WorldToCell(destination.transform.position);
            destination.Grid.ShowHintTilesFor(entity.occupant, (Vector2Int)cell);

            using (ListPool<Violation>.Get(out var violations))
            {
                if (!_occupancyManager.TryGetSimulatedViolations(entity.occupant, destination, violations))
                    return;

                destination.Grid.ShowViolatedTiles(violations);
            }
        }
        public void OnHover(Draggable draggable)
        {
            if (!draggable.TryGetComponent(out Entity entity))
            {
                Debug.LogError($"Draggable {draggable.name} does not have an Entity component.");
                return;
            }

            _hud.UpdateCard(entity);

            if (entity.occupant.Grid == null)
                return;

            using (ListPool<Violation>.Get(out var violations))
            {
                if (!_occupancyManager.GetViolationsFor(entity.occupant, violations))
                    return;

                entity.occupant.Grid.ShowViolatedTiles(violations);
            }
        }
        public void OnHoverExit(Draggable draggable)
        {
            _hud.HideCard();
            _occupancyManager.HideAllTileMarkers();
        }
        public bool TryDropEntity(Entity entity, Occupiable destination)
        {
            if (entity == null)
                return false;

            return _occupancyManager.TryMoveOccupant(entity.occupant, destination);
        }

        void UpdateViolationDialogues()
        {
            if (_levelSystem.Judge())
            {
                _dialogueSystem.HideAllDialogues();
                _activeDialogueTargets.Clear();
                return;
            }

            var violations = _occupancyManager.GetViolations();
            using (HashSetPool<Transform>.Get(out var nextTargets))
            {
                foreach (var violation in violations)
                    nextTargets.Add(violation.Occupant.transform);

                foreach (var activeTarget in _activeDialogueTargets)
                {
                    if (!nextTargets.Contains(activeTarget))
                        _dialogueSystem.HideDialogue(activeTarget);
                }

                _activeDialogueTargets.Clear();
                foreach (var target in nextTargets)
                    _activeDialogueTargets.Add(target);
            }

            foreach (var violation in violations)
                _dialogueSystem.ShowDialogue(violation.Occupant.transform, violation.Rule.ErrorDialogue);
        }
    }
}
