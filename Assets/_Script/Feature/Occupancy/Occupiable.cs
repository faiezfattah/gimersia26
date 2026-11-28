using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    [SelectionBase]
    public class Occupiable : MonoBehaviour
    {
        public TileTraitFlags Traits;
        public SpriteRenderer AvailableMarker;
        public SpriteRenderer ViolatedMarker;
        public SpriteRenderer HintMarker;
        public SpriteRenderer DefaultMarker;
        public OccupancyGrid Grid { get; private set; }
        internal void Inject(OccupancyGrid grid)
        {
            Grid = grid;
        }
        public void ShowViolatedMarker()
        {
            HideAllMarkers();
            ViolatedMarker.enabled = true;
            DefaultMarker.enabled = false;
        }
        public void HideViolatedMarker()
        {
            ViolatedMarker.enabled = false;
            DefaultMarker.enabled = true;
        }

        public void ShowAvailableMarker()
        {
            HideAllMarkers();
            AvailableMarker.enabled = true;
            DefaultMarker.enabled = false;
        }
        
        public void HideAvailableMarker()
        {
            AvailableMarker.enabled = false;
            DefaultMarker.enabled = true;
        }
        
        // display the rule scope and extent
        public void ShowHintMarker()
        {
            HideAllMarkers();
            HintMarker.enabled = true;
            DefaultMarker.enabled = false;
        }
        public void HideHintMarker()
        {
            HintMarker.enabled = false;
            DefaultMarker.enabled = true;
        }

        public void HideAllMarkers()
        {
            HideAvailableMarker();
            HideViolatedMarker();
            HideHintMarker();
            DefaultMarker.enabled = true;
        }
    }
}