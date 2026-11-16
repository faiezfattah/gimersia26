using Reflex.Attributes;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [Inject] OccupancyManager _judge;
    [Inject] DialogueSystem _dialogueSystem;
    public void EvaluateLevel()
    {
        Debug.Assert(_judge != null, "OccupancyJudge is not injected.");
        Debug.Assert(_dialogueSystem != null, "DialogueSystem is not injected.");

        var violations = _judge.Judge();
        if (violations != null & violations.Count > 0)
        {
            foreach (var violation in violations)
            {
                Debug.Log($"Violation: {violation}");
                _dialogueSystem.ShowDialogue(violation.Occupant.transform, violation.Rule.ErrorText);
            }
        }
        else
        {
            Debug.Log("Level passed!");
        }
    }

    public void NextLevel()
    {
        // spawn the new bus
        // get the grid 
        // and it to the judging system
        // remove the old grid from judging system
    }
}
