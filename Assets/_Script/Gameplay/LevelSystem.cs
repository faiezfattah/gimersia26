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

        int _currentLevelIndex = 0;
        
        Transform _currentBus;
        EntitySpawner _spawner = new();

        public bool EvaluateLevel()
        {
            Debug.Assert(_judge != null, "OccupancyJudge is not injected.");

            var verdict = _judge.HasViolations();

            if (verdict) Debug.Log("Level failed bro");
            else Debug.Log("Level passed bro");

            return verdict;
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
            // clean up current state
            _currentLevelIndex++;
            // prepare the next level
            
            // spawn the new bus
            // get the grid 
        }
    }
}