using System.Collections.Generic;
using Alchemy.Inspector;
using Gimersia.Feature.Occupancy;
using UnityEngine;
using UnityEngine.Pool;

public class OccupancyManager
{
    [SerializeField, ReadOnly] List<OccupancyGrid> _grids = new();

    public OccupancyGrid GetOverworldGrid()
    {
        foreach (var grid in _grids)
        {
            if (grid.gridType == GridType.Overworld) return grid;
        }
        return null;
    }

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

    public List<Violation> GetViolations()
    {
        var v = new List<Violation>();

        foreach (var grid in _grids)
        {
            grid.GetViolations(v);
        }
        
        return v;
    }
    public bool HasViolations()
    {
        using (ListPool<Violation>.Get(out var violations))
        {
            foreach (var grid in _grids)
            {
                grid.GetViolations(violations);
            }
            return violations.Count == 0;
        }
    }
    public void ShowDroppableTiles()
    {
        foreach (var grid in _grids)
        {
            grid.ShowDroppableTiles();
        }
    }
    public void HideDroppableTiles()
    {
        foreach (var grid in _grids)
        {
            grid.HideDroppableTiles();
        }
    }
}
