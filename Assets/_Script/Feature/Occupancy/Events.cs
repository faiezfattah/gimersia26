using Oneiric.Core.EventBus;
using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    public readonly struct OccupyEvent : IEvent
    {
        public readonly OccupancyGrid Grid;
        public readonly Occupant Occupant;

        public OccupyEvent(OccupancyGrid grid, Occupant occupant)
        {
            Grid = grid;
            Occupant = occupant;
        }
    }
    public readonly struct UnoccupyEvent : IEvent
    {
        public readonly OccupancyGrid Grid;
        public readonly Occupant Occupant;

        public UnoccupyEvent(OccupancyGrid grid, Occupant occupant)
        {
            Grid = grid;
            Occupant = occupant;
        }
    }
    public readonly struct OccupantMovedEvent : IEvent
    {
        public readonly Occupant Occupant;
        public readonly OccupancyGrid OldGrid;
        public readonly Vector2Int OldPosition;
        public readonly OccupancyGrid NewGrid;
        public readonly Vector2Int NewPosition;

        public OccupantMovedEvent(Occupant occupant, OccupancyGrid oldGrid, Vector2Int oldPosition, OccupancyGrid newGrid, Vector2Int newPosition)
        {
            Occupant = occupant;
            OldGrid = oldGrid;
            OldPosition = oldPosition;
            NewGrid = newGrid;
            NewPosition = newPosition;
        }
    }
}