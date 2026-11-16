using System;
using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    public class Occupiable : MonoBehaviour
    {
        public TileTraitFlags Traits;
        public SpriteRenderer AvailableMarker;
        public OccupancyGrid Grid { get; private set; }
        
        internal void Inject(OccupancyGrid grid)
        {
            Grid = grid;
        }
        public void ShowAvailableMarker()
        {
            AvailableMarker.enabled = true;
        }
        public void HideAvailableMarker()
        {
            AvailableMarker.enabled = false;
        }
    }
}