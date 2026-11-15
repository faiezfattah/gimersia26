using System;
using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    public class Occupiable : MonoBehaviour
    {
        public TileTraitFlags Traits;   // we show this as icons
        public OccupancyGrid Grid { get; private set; }
        
        internal void Inject(OccupancyGrid grid)
        {
            Grid = grid;
        }
    }
}