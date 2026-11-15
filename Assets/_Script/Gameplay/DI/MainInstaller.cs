using Gimersia.Feature.Occupancy;
using Gimersia.Gameplay.DragAndDrop;
using Reflex.Core;
using UnityEngine;

using Lifetime = Reflex.Enums.Lifetime;
using Resolution = Reflex.Enums.Resolution;

namespace Gimersia.Gameplay.DI {
    public class MainInstaller : MonoBehaviour, IInstaller {
        [SerializeField] OccupancyGrid _occupancyGrid;
        public void InstallBindings(ContainerBuilder builder)
        {
            builder
                .RegisterValue(_occupancyGrid);
        }
    }
}