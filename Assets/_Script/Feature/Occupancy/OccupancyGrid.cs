using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gimersia.Feature.Occupancy
{
    public class OccupancyGrid : MonoBehaviour
    {
        public Tilemap tilemap;

        [ShowInInspector] 
        Dictionary<Vector2Int, Occupiable> _occupancyGrid = new();

        [ShowInInspector] 
        Dictionary<Vector2Int, OccupantData> _occupants = new();
        void Start()
        {
            DiscoverOccupiable();
        }
        [Button]
        void DiscoverOccupiable()
        {
            _occupancyGrid.Clear();    
            foreach (var t in tilemap.GetComponentsInChildren<Occupiable>())
            {
                var pos = tilemap.WorldToCell(t.transform.position);
                _occupancyGrid[(Vector2Int)pos] = t;
            }
        }
        [Button]
        public void ClearOccupants()
        {
            _occupants.Clear();
        }

        public bool Occupy(OccupantData occupant, Vector2Int position)
        {
            _occupants[position] = occupant;

            return false;
        }

        public bool IsOccupied(Vector2Int position)
        {
            return _occupancyGrid.TryGetValue(position, out var occupiable);
        }

        public void GetViolation(List<Violation> into)
        {
            into.Clear();

            foreach (var (cell, occupant) in _occupants)
            {
                foreach (var rule in occupant.Rules)
                {
                    var offenders = new List<Vector2Int>();

                    foreach (var scopedCell in GetCellInScope(cell, rule.Scope, rule.Extent))
                    {
                        var subjectFlags = ResolveSubjectFlags(rule.Subject, scopedCell);
                        
                        switch (rule.Polarity, subjectFlags & rule.Target)
                        {
                            case (Polarity.Require, 0):
                                offenders.Add(scopedCell);
                                break;
                            case (Polarity.Avoid, _):
                                offenders.Add(scopedCell);
                                break;
                            default:
                                Debug.Log($"Rule {rule.name} satisfied for {occupant.name} at {cell} with subject {rule.Subject} and scope {rule.Scope}");
                                break;
                        }
                    }
 
                    if (offenders.Count > 0)
                    {
                        into.Add(new Violation(cell, occupant, rule, offenders));
                    }
                }
            }
        }
        TraitFlags ResolveSubjectFlags(Subject subject, Vector2Int cell)
        {
            return subject switch
            {
                Subject.Entity => _occupants[cell].Traits,
                Subject.Tile => _occupancyGrid[cell].Type switch
                    {
                        OccupiableType.Land => TraitFlags.Land,
                        OccupiableType.Water => TraitFlags.Water,
                        _ => throw new System.Exception("unrecognizeable type")
                    },
                _ => throw new System.Exception("unrecognizeable type")
            };
        } 
        public IEnumerable<Vector2Int> GetCellInScope(Vector2Int origin, Scope scope, int extent)
        {
            switch (scope)
            {
                case Scope.Self:
                    yield return origin;
                    break;
                default:
                    throw new System.Exception("unrecognizeable type");
            }
        }
    }
    
    public readonly struct Violation
    {
        public readonly Vector2Int Cell;
        public readonly OccupantData Occupant;
        public readonly Rule Rule;
        public readonly List<Vector2Int> Offenders; // cells that matched (useful for Avoid)
        public Violation(Vector2Int c, OccupantData o, Rule r, List<Vector2Int> off)
        { 
            Cell = c; 
            Occupant = o; 
            Rule = r; 
            Offenders = off; 
        }
    }
}