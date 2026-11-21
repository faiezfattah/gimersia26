using Cysharp.Threading.Tasks;
using Gimersia.Feature.Screens;
using Reflex.Attributes;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;

using static UnityEngine.Object;

namespace Gimersia.Gameplay
{
    [SourceGeneratorInjectable]
    public partial class LevelSystem
    {
        [Inject] OccupancyManager _judge;
        [Inject] LevelData[] _levelDatas;
        [Inject] Container _diContainer;
        [Inject] ScreenManager _screens;

        int _currentLevelIndex = 0;

        CarAnimation _currentBus;
        EntitySpawner _spawner = new();

        public void Submit()
        {
            if (!Judge()) return;

            NextLevel().Forget(); ;
        }
        public bool Judge()
        {
            var hasViolation = _judge.HasViolations();

            Debug.Log($"Level {_currentLevelIndex} hasViolation: {hasViolation}");
            if (hasViolation)
            {
                foreach (var violation in _judge.GetViolations())
                {
                    Debug.Log($"Violation: {violation.Rule.ErrorText} for {violation.Cell}");
                }
            }
            return !hasViolation;
        }
        public void StartLevel()
        {
            var data = _levelDatas[_currentLevelIndex];
            _spawner.Spawn(_judge.GetOverworldGrid(), data);
            _currentBus = Instantiate(data.busPrefab);
            _currentBus.AnimateIn();
            GameObjectInjector.InjectRecursive(_currentBus.gameObject, _diContainer);
        }

        public async UniTaskVoid NextLevel()
        {
            await _currentBus.AnimateOut();
            Destroy(_currentBus.gameObject);
            _currentLevelIndex++;
            if (_currentLevelIndex < _levelDatas.Length)
            {
                StartLevel();
                return;
            }

            _screens.ShowScreen("Win");
        }
    }
}