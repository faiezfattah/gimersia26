using System.Collections.Generic;
using Alchemy.Inspector;
using Gimersia.Feature.Occupancy;
using UnityEngine;
using UnityEngine.Pool;

public class OccupancyJudge
{
    [SerializeField, ReadOnly] List<OccupancyGrid> _grids = new();

    public void AddGrid(OccupancyGrid grid)
    {
        if (!_grids.Contains(grid)) _grids.Add(grid);
        else Debug.LogError($"Grid {grid.name} is already registered in JudgingSystem.");
    }
    public void RemoveGrid(OccupancyGrid grid)
    {
        if (_grids.Contains(grid)) _grids.Remove(grid);
        else Debug.LogError($"Grid {grid.name} is not registered in JudgingSystem.");
    }

    public List<Violation> Judge()
    {
        var v = new List<Violation>();

        foreach (var grid in _grids)
        {
            grid.GetViolations(v);
        }
        
        return v;
    }
}
