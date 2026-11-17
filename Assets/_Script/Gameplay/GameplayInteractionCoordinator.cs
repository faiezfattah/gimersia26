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
            _levelSystem.EvaluateLevel();

            foreach (var violation in _occupancyManager.GetViolations())
            {
                _dialogueSystem.ShowDialogue(
                    violation.Occupant.transform,
                    violation.Rule.ErrorText);
            }
        }
    }
}
