using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    public class Occupiable : MonoBehaviour
    {
        public TileTraitFlags Traits;
        public SpriteRenderer AvailableMarker;
        public SpriteRenderer ViolatedMarker;
        public SpriteRenderer HintMarker;
        public OccupancyGrid Grid { get; private set; }
        
        internal void Inject(OccupancyGrid grid)
        {
            Grid = grid;
        }
        public void ShowViolatedMarker()
        {
            HideAllMarkers();
            ViolatedMarker.enabled = true;
        }
        public void HideViolatedMarker()
        {
            ViolatedMarker.enabled = false;
        }

        public void ShowAvailableMarker()
        {
            HideAllMarkers();
            AvailableMarker.enabled = true;
        }
        public void HideAvailableMarker()
        {
            AvailableMarker.enabled = false;
        }
        public void ShowHintMarker()
        {
            HideAllMarkers();
            Debug.Log($"Showing hint marker for {name}");
            HintMarker.enabled = true;
        }
        public void HideHintMarker()
        {
            HintMarker.enabled = false;
        }

        public void HideAllMarkers()
        {
            HideAvailableMarker();
            HideViolatedMarker();
            HideHintMarker();
        }
    }
}