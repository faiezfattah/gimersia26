using UnityEngine;
using Alchemy.Inspector;
using System.Collections.Generic;
using System;

namespace Gimersia.Feature.Occupancy
{
    public sealed class Occupant : MonoBehaviour
    {
        public Nullable<Vector2Int> Position {internal set; get;}
        public OccupancyGrid Grid { internal set; get; }
        [Required] public OccupantData Data;
        public List<Rule> Rules;
    }
}
