using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using Gimersia.Feature.Occupancy;
using Oneiric.Core.EventBus;
using UnityEngine;
using UnityEngine.Pool;

public class OccupancyManager
{
    [SerializeField, ReadOnly] List<OccupancyGrid> _grids = new();

    Occupant _pickedOccupant;
    OccupancyGrid _pickedSourceGrid;
    Vector2Int? _pickedSourcePosition;

    public OccupancyGrid GetOverworldGrid()
    {
        foreach (var grid in _grids)
        {
            if (grid.gridType == GridType.Overworld) return grid;
        }
        return null;
    }

    public List<OccupancyGrid> GetStageGrid()
    {
        var stageGrids = new List<OccupancyGrid>();
        foreach (var grid in _grids)
        {
            if (grid.gridType == GridType.Stage) stageGrids.Add(grid);
        }
        return stageGrids;
    }
    public void GetOccupants(List<Occupant> into)
    {
        foreach (var grid in _grids)
        {
            grid.GetOccupants(into);
        }
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

    public bool PickUpOccupant(Occupant occupant)
    {
        if (occupant == null)
            return false;

        _pickedOccupant = occupant;
        _pickedSourceGrid = occupant.Grid;
        _pickedSourcePosition = occupant.Position;

        if (_pickedSourceGrid != null && _pickedSourcePosition.HasValue)
            return _pickedSourceGrid.TryVacate(occupant);

        return true;
    }

    public bool CancelPickUp(Occupant occupant)
    {
        if (occupant == null || occupant != _pickedOccupant)
            return false;

        var sourceGrid = _pickedSourceGrid;
        var sourcePosition = _pickedSourcePosition;
        ClearPickedState();

        if (sourceGrid != null && sourcePosition.HasValue)
            return sourceGrid.TryOccupy(occupant, sourcePosition.Value);

        return false;
    }

    public bool TryMoveOccupant(Occupant occupant, Occupiable destination)
    {
        if (occupant == null || destination == null || destination.Grid == null)
        {
            Debug.LogError($"Invalid occupant or destination. Occupant: {occupant}, Destination: {destination}");
            return false;
        }

        var destinationGrid = destination.Grid;
        var destinationPosition = (Vector2Int)destinationGrid.tilemap.WorldToCell(destination.transform.position);

        var wasPicked = occupant == _pickedOccupant;
        var sourceGrid = wasPicked ? _pickedSourceGrid : occupant.Grid;
        var sourcePosition = wasPicked ? _pickedSourcePosition : occupant.Position;

        if (!destinationGrid.IsVacant(destinationPosition))
        {
            if (!wasPicked && occupant.Grid == destinationGrid && occupant.Position == destinationPosition)
            {
                EventBus.Raise(new OccupantMovedEvent(occupant, sourceGrid, destinationPosition, destinationGrid, destinationPosition));
                return true;
            }
            return false;
        }

        if (!wasPicked && sourceGrid != null && sourcePosition.HasValue)
        {
            if (!sourceGrid.TryVacate(occupant))
                return false;
        }

        if (destinationGrid.TryOccupy(occupant, destinationPosition))
        {
            if (wasPicked) ClearPickedState();
            EventBus.Raise(new OccupantMovedEvent(
                occupant,
                sourceGrid,
                sourcePosition ?? destinationPosition,
                destinationGrid,
                destinationPosition));
            return true;
        }

        if (!wasPicked && sourceGrid != null && sourcePosition.HasValue)
            sourceGrid.TryOccupy(occupant, sourcePosition.Value);
            
        return false;
    }

    void ClearPickedState()
    {
        _pickedOccupant = null;
        _pickedSourceGrid = null;
        _pickedSourcePosition = null;
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
    public bool GetViolationsFor(Occupant occupant, List<Violation> into)
    {
        foreach (var grid in _grids)
        {
            grid.GetViolations(occupant, into);
        }
        return into.Count > 0;
    }
    public bool TryGetSimulatedViolations(Occupant occupant, Occupiable destination, List<Violation> into)
    {
        if (occupant == null || destination == null || destination.Grid == null)
            return false;

        return destination.Grid.TryGetSimulatedViolations(occupant, destination, into);
    }
    public void HideAllTileMarkers()
    {
        foreach (var grid in _grids)
        {
            grid.HideAllMarkers();
        }
    }
    public bool HasViolations()
    {
        using (ListPool<Violation>.Get(out var violations))
        {
            foreach (var grid in _grids)
            {
                grid.GetViolations(violations);
            }
            return violations.Count > 0;
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
