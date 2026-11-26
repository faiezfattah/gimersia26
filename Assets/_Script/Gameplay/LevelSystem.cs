using Cysharp.Threading.Tasks;
using Gimersia.Feature.Occupancy;
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
        [Inject] CutsceneSystem _cutscene;

        int _currentLevelIndex = 0;

        CarAnimation _currentBus;
        OccupancyGrid _overworld;
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
                    Debug.Log($"Violation: {violation.Rule.ErrorDialogue} for {violation.Cell}");
                }
            }
            return !hasViolation;
        }
        public async UniTask StartLevel()
        {
            var data = _levelDatas[_currentLevelIndex];
            
            _overworld = Instantiate(data.OverworldPrefab);
            GameObjectInjector.InjectRecursive(_overworld.gameObject, _diContainer);
            _overworld.transform.position = Vector3.zero;
            
            _currentBus = Instantiate(data.BusPrefab);
            GameObjectInjector.InjectRecursive(_currentBus.gameObject, _diContainer);

            _spawner.Spawn(_overworld, data);

            await _cutscene.PlayIntro(_currentBus, data.DialogueCommands);
            await _cutscene.RunDialogueCommand(data.DialogueCommands);
            _cutscene.EndIntro();
        }

        public async UniTaskVoid NextLevel()
        {
            await _currentBus.AnimateOut();
            Destroy(_currentBus.gameObject);
            Destroy(_overworld.gameObject);
            
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