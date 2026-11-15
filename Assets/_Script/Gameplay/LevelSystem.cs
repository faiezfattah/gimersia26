using Reflex.Attributes;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [Inject] OccupancyJudge _judge;
    
    public void EvaluateLevel()
    {
        var violations = _judge.Judge();
        if (violations != null & violations.Count > 0) 
            Debug.Log(violations);
    }

    public void NextLevel()
    {
        // spawn the new bus
        // get the grid 
        // and it to the judging system
        // remove the old grid from judging system
    }
}
