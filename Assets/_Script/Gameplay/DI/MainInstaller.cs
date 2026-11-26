using System;
using Gimersia.Core;
using Gimersia.Feature.Blackbar;
using Gimersia.Feature.Screens;
using Reflex.Core;
using UnityEngine;

using Lifetime = Reflex.Enums.Lifetime;
using Resolution = Reflex.Enums.Resolution;

namespace Gimersia.Gameplay.DI {
    public class MainInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] HUDManager _hudManager;
        [SerializeField] DialogueSystem _dialogueSystem;
        [SerializeField] LevelData[] _levelDatas;
        [SerializeField] ScreenManager _screens;
        [SerializeField] Camera _camera;
        [SerializeField] BlackbarManager _blackbar;
        public void InstallBindings(ContainerBuilder builder)
        {
            builder
                .RegisterValue(_hudManager)
                .RegisterValue(_dialogueSystem)
                .RegisterValue(_levelDatas)
                .RegisterValue(_screens)
                .RegisterValue(_camera)
                .RegisterValue(_blackbar)
                .RegisterType(typeof(LevelSystem), Lifetime.Singleton, Resolution.Eager)
                .RegisterType(typeof(GameplayInteractionCoordinator), Lifetime.Singleton, Resolution.Eager)
                .RegisterType(typeof(OccupancyManager), Lifetime.Singleton, Resolution.Eager)
                .RegisterType(typeof(InputMap), Lifetime.Singleton, Resolution.Eager)
                .RegisterType(typeof(InputMapLifecycle), Lifetime.Singleton, Resolution.Eager)
                .RegisterType(typeof(CutsceneSystem), Lifetime.Singleton, Resolution.Lazy);
        }
    }
    class InputMapLifecycle : IDisposable {
        InputMap _map;
        public InputMapLifecycle(InputMap inputMap)
        {
            _map = inputMap;
            _map.UI.Enable();
            _map.Main.Enable();
            _map.Enable();
        }

        public void Dispose()
        {
            _map.UI.Disable();
            _map.Main.Disable();
            _map.Disable();
        }
    }
}
