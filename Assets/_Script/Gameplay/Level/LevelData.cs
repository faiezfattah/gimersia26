using System.Collections.Generic;
using Gimersia.Feature.Occupancy;
using UnityEngine;

namespace Gimersia.Gameplay
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Gimersia/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public CarAnimation BusPrefab;
        public OccupancyGrid OverworldPrefab;
        public List<Entity> EntitiesPrefabs;
        public string[] DialogueCommands;
    }
}
