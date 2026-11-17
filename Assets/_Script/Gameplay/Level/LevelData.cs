using System.Collections.Generic;
using UnityEngine;

namespace Gimersia.Gameplay
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Gimersia/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public Transform busPrefab;
        public List<Entity> entitiesPrefabs;
    }
}