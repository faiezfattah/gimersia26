using System.Collections.Generic;
using Alchemy.Inspector;
using Reflex.Attributes;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gimersia.Feature.Occupancy
{
    public enum GridType
    {
        Overworld,
        Stage
    }
    [SourceGeneratorInjectable]
    public partial class OccupancyGrid : MonoBehaviour
    {
        OccupancyManager _manager;

        public Tilemap tilemap;
        public GridType gridType;

        [ShowInInspector] Dictionary<Vector2Int, Occupiable> _occupancyGrid = new();
        [ShowInInspector] Dictionary<Vector2Int, Occupant> _occupants = new();

        [Inject]
        void Inject(OccupancyManager manager)
        {
            Debug.Log($"OccupancyGrid Injected with manager: {manager}");
            _manager = manager;
            _manager.AddGrid(this);
            DiscoverOccupiable();
        }
        void OnDestroy()
        {
            if (_manager != null) _manager.RemoveGrid(this);
        }
        [Button]
        void DiscoverOccupiable()
        {
            _occupancyGrid.Clear();
            foreach (var t in tilemap.GetComponentsInChildren<Occupiable>())
            {
                var pos = tilemap.WorldToCell(t.transform.position);
                _occupancyGrid[(Vector2Int)pos] = t;
                t.Inject(this);
            }
        }
        [Button]
        public void ClearOccupants()
        {
            foreach (var occupant in _occupants.Values)
            {
                occupant.Position = null;
                occupant.Grid = null;
            }
            _occupants.Clear();
        }

        /// <returns>return false if already occupied</returns>
        public bool TryOccupy(Occupant occupant, Vector2Int position)
        {
            if (_occupants.ContainsKey(position))
                return false;

            _occupants[position] = occupant;
            occupant.transform.SetParent(tilemap.transform, false);
            occupant.transform.position = tilemap.GetCellCenterWorld((Vector3Int)position);

            occupant.Position = position;
            occupant.Grid = this;

            return true;
        }
        public bool TryOccupy(Occupant occupant, Vector3 worldPosition)
        {
            var cell = tilemap.WorldToCell(worldPosition);
            return TryOccupy(occupant, (Vector2Int)cell);
        }
        public bool TryOccupy(Occupant occupant, Occupiable occupiable)
        {
            var cell = tilemap.WorldToCell(occupiable.transform.position);
            return TryOccupy(occupant, (Vector2Int)cell);
        }
        public bool TryVacate(Occupant occupant)
        {
            if (!occupant.Position.HasValue) return false;
            var pos = occupant.Position.Value;
            if (!_occupants.TryGetValue(pos, out var current) || current != occupant) return false;

            _occupants.Remove(pos);
            occupant.Position = null;
            if (occupant.Grid == this)
                occupant.Grid = null;

            Debug.Assert(_occupants.ContainsKey(pos) == false, $"Occupant {occupant.name} was not removed from the grid {name}.");
            return true;
        }
        public bool IsVacant(Vector2Int position)
        {
            return !_occupants.ContainsKey(position);
        }
        public bool TryGetVacantTiles(List<Vector2Int> into)
        {
            var hasVacant = false;
            foreach (var kvp in _occupancyGrid)
            {
                if (!_occupants.ContainsKey(kvp.Key))
                {
                    into.Add(kvp.Key);
                    hasVacant = true;
                }
            }
            
            return hasVacant;
        }

        public bool IsOccupied(Vector2Int position)
        {
            return _occupancyGrid.TryGetValue(position, out var tile) && _occupants.ContainsKey(position);
        }
        public bool TryGetTileAt(Vector2Int position, out Occupiable occupiable)
        {
            return _occupancyGrid.TryGetValue(position, out occupiable);
        }

        /// <returns>true if there is a violation</returns>
        public bool GetViolations(List<Violation> into)
        {
            var hasViolation = false;

            foreach (var (cell, occupant) in _occupants)
            {
                foreach (var rule in occupant.Rules)
                {
                    var offenders = new List<Vector2Int>();

                    foreach (var scopedCell in GetCellInScope(cell, rule.Scope, rule.Extent))
                    {
                        var (entity, tile) = ResolveSubjectFlags(rule.Subject, scopedCell);

                        switch (rule.Polarity, entity & rule.TargetEntity, tile & rule.TargetTile)
                        {
                            case (Polarity.Require, 0, 0):
                            case (Polarity.Avoid, not 0, _):
                            case (Polarity.Avoid, _, not 0):
                                offenders.Add(scopedCell);
                                break;
                            default:
                                break;
                        }
                    }

                    if (offenders.Count > 0)
                    {
                        into.Add(new Violation(cell, occupant, rule, offenders));
                        hasViolation = true;
                    }
                }
            }
            return hasViolation;
        }

        (EntityTraitFlags, TileTraitFlags) ResolveSubjectFlags(Subject subject, Vector2Int cell)
        {
            EntityTraitFlags entity = 0;
            TileTraitFlags tile = 0;

            if ((subject & Subject.Entity) != 0 && _occupants.TryGetValue(cell, out var o))
                entity = o.Data.Traits;
            if ((subject & Subject.Tile) != 0 && _occupancyGrid.TryGetValue(cell, out var t))
                tile = t.Traits;

            if (subject == 0) throw new System.ArgumentException($"Unrecognized subject: {subject}");

            return (entity, tile);
        }

        IEnumerable<Vector2Int> GetCellInScope(Vector2Int origin, Scope scope, int extent)
        {
            switch (scope)
            {
                case Scope.Self:
                    yield return origin;
                    break;
                case Scope.Surrounding:
                    for (int x = -extent; x <= extent; x++)
                    {
                        for (int y = -extent; y <= extent; y++)
                        {
                            if (x == 0 && y == 0) continue;
                            yield return origin + new Vector2Int(x, y);
                        }
                    }
                    break;
                case Scope.Adjacent:
                    for (int i = -extent; i <= extent; i++)
                    {
                        if (i == 0) continue;
                        yield return origin + new Vector2Int(i, 0);
                        yield return origin + new Vector2Int(0, i);
                    }
                    break;
                case Scope.Column:
                    for (int y = -extent; y <= extent; y++)
                    {
                        if (y == 0) continue;
                        yield return origin + new Vector2Int(0, y);
                    }
                    break;
                case Scope.Row:
                    for (int x = -extent; x <= extent; x++)
                    {
                        if (x == 0) continue;
                        yield return origin + new Vector2Int(x, 0);
                    }
                    break;
                default:
                    throw new System.Exception("not yet supported scope: " + scope);
            }
        }
        public void ShowDroppableTiles()
        {
            foreach (var occupiable in _occupancyGrid.Keys)
            {
                if (IsVacant(occupiable))
                    _occupancyGrid[occupiable].ShowAvailableMarker();
                else
                    _occupancyGrid[occupiable].HideAvailableMarker();
            }
        }
        public void HideDroppableTiles()
        {
            foreach (var occupiable in _occupancyGrid.Keys)
            {
                _occupancyGrid[occupiable].HideAvailableMarker();
            }
        }
    }
}