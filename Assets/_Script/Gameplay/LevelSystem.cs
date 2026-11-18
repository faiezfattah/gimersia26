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
        [Inject] WinScreen _winScreen;

        int _currentLevelIndex = 0;

        Transform _currentBus;
        EntitySpawner _spawner = new();

        public void Submit()
        {
            if (!Judge()) return;

            NextLevel();
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
            GameObjectInjector.InjectRecursive(_currentBus.gameObject, _diContainer);
        }

        public void NextLevel()
        {
            Destroy(_currentBus.gameObject);
            _currentLevelIndex++;
            if (_currentLevelIndex < _levelDatas.Length)
            {
                StartLevel();
                return;
            }

            _winScreen.Show();
        }
    }
}