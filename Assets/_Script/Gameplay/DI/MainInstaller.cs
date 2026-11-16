using Gimersia.Feature.Occupancy;
using Reflex.Core;
using UnityEngine;

using Lifetime = Reflex.Enums.Lifetime;
using Resolution = Reflex.Enums.Resolution;

namespace Gimersia.Gameplay.DI {
    public class MainInstaller : MonoBehaviour, IInstaller {
        [SerializeField] HUDManager _hudManager;
        [SerializeField] DialogueSystem _dialogueSystem;
        public void InstallBindings(ContainerBuilder builder)
        {
            builder
                .RegisterValue(_hudManager)
                .RegisterValue(_dialogueSystem)
                .RegisterType(typeof(OccupancyJudge), Lifetime.Singleton, Resolution.Eager);
        }
    }
}