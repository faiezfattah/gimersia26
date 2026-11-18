using Gimersia.Feature.Occupancy;
using Reflex.Attributes;

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
           if (_levelSystem.Judge()) return;

            foreach (var violation in _occupancyManager.GetViolations())
                _dialogueSystem.ShowDialogue(violation.Occupant.transform, violation.Rule.ErrorText);
        }

        public bool TryMove(Occupant occupant, Occupiable destination)
        {
            return _occupancyManager.TryMove(occupant, destination);
        }
    }
}
