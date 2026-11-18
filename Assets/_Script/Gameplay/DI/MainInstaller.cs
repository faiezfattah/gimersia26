using Gimersia.Feature.Occupancy;
using Reflex.Core;
using UnityEngine;

using Lifetime = Reflex.Enums.Lifetime;
using Resolution = Reflex.Enums.Resolution;

namespace Gimersia.Gameplay.DI {
    public class MainInstaller : MonoBehaviour, IInstaller {
        [SerializeField] HUDManager _hudManager;
        [SerializeField] DialogueSystem _dialogueSystem;
        [SerializeField] LevelData[] _levelDatas;
        [SerializeField] WinScreen _winScreen;
        public void InstallBindings(ContainerBuilder builder)
        {
            builder
                .RegisterValue(_hudManager)
                .RegisterValue(_dialogueSystem)
                .RegisterValue(_levelDatas)
                .RegisterValue(_winScreen)
                .RegisterType(typeof(LevelSystem), Lifetime.Singleton, Resolution.Eager)
                .RegisterType(typeof(GameplayInteractionCoordinator), Lifetime.Singleton, Resolution.Eager)
                .RegisterType(typeof(OccupancyManager), Lifetime.Singleton, Resolution.Eager);
        }
    }
}