using System.Collections.Generic;
using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    public readonly struct Violation
    {
        public readonly Vector2Int Cell;
        public readonly Occupant Occupant;
        public readonly Rule Rule;
        public readonly List<Vector2Int> Offenders; // cells that matched (useful for Avoid)

        public Violation(Vector2Int c, Occupant o, Rule r, List<Vector2Int> off)
        {
            Cell = c;
            Occupant = o;
            Rule = r;
            Offenders = off;
        }
        public override string ToString()
        {
            return $"at {Cell} violates {Rule}";
        }
    }
}