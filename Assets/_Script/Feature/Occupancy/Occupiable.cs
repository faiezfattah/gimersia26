using System;
using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    public class Occupiable : MonoBehaviour
    {
        public TileTraitFlags Traits;
        public SpriteRenderer AvailableMarker;
        public SpriteRenderer ViolatedMarker;
        public OccupancyGrid Grid { get; private set; }
        
        internal void Inject(OccupancyGrid grid)
        {
            Grid = grid;
        }
        public void ShowViolatedMarker()
        {
            ViolatedMarker.enabled = true;
        }
        public void HideViolatedMarker()
        {
            ViolatedMarker.enabled = false;
        }

        public void ShowAvailableMarker()
        {
            AvailableMarker.enabled = true;
        }
        public void HideAvailableMarker()
        {
            AvailableMarker.enabled = false;
        }

        public void HideAllMarkers()
        {
            HideAvailableMarker();
            HideViolatedMarker();
        }
    }
}