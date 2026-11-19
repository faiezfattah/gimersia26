using Cysharp.Threading.Tasks.Triggers;
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

        public void OnPickUp(Entity entity)
        {
            _dialogueSystem.HideDialogue(entity.occupant.transform);
            _hud.UpdateCard(entity);
            _occupancyManager.ShowDroppableTiles();
        }

        public void OnDrop()
        {
            _occupancyManager.HideDroppableTiles();
            _occupancyManager.HideViolatedTiles();
            if (_levelSystem.Judge()) return;

            foreach (var violation in _occupancyManager.GetViolations())
                _dialogueSystem.ShowDialogue(violation.Occupant.transform, violation.Rule.ErrorText);
        }
        public void OnDrag(Draggable draggable, Droppable droppable)
        {
            if (!draggable.TryGetComponent(out Entity entity))
            {
               Debug.LogError($"Draggable {draggable.name} does not have an Entity component.");
               return; 
            }

            _occupancyManager.HideViolatedTiles();

            if (droppable == null ||
                !droppable.TryGetComponent(out Occupiable destination))
            {
                return;
            }

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

            using (ListPool<Violation>.Get(out var violations))
            {
                if (!_occupancyManager.GetViolationsFor(entity.occupant, violations))
                    return;

                entity.occupant.Grid.ShowViolatedTiles(violations);
            }
        }
        public void OnHoverExit(Draggable draggable)
        {
            _occupancyManager.HideViolatedTiles();
        }
        public bool TryDropEntity(Entity entity, Occupiable destination)
        {
            if (entity == null)
                return false;

            return _occupancyManager.TryMoveOccupant(entity.occupant, destination);
        }
    }
}
