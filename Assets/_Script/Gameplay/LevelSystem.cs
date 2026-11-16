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

        if (_judge.HasViolations())
        {
            Debug.Log("Level failed bro");
        }
        else 
        {
            Debug.Log("Level passed bro");
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
