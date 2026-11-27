using UnityEngine;
using System.Collections.Generic;
using System;

namespace Gimersia.Feature.Occupancy
{
    public sealed class Occupant : MonoBehaviour
    {
        public Nullable<Vector2Int> Position { internal set; get; }
        public OccupancyGrid Grid { internal set; get; }
        public int Width = 1;
        public int Height = 1;

        [Space(10)]
        public EntityTraitFlags Traits;
        public List<Rule> Rules = new();
    }
}
