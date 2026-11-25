using System.Collections.Generic;
using Gimersia.Feature.Occupancy;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Object;

namespace Gimersia.Gameplay
{
    public class EntitySpawner
    {
        public void Spawn(OccupancyGrid grid, LevelData levelData)
        {
            // get all occupiable tiles from the grid
            // spawn entities on them
            using (ListPool<Vector2Int>.Get(out var vacantTiles))
            {
                if (!grid.TryGetVacantTiles(vacantTiles))
                {
                    Debug.LogError("No vacant tiles available for spawning entities.");
                    return;
                }

                foreach (var entityPrefab in levelData.EntitiesPrefabs)
                {
                    var entity = Instantiate(entityPrefab);
                    var occupiableTile = GetRandomVacantTile(vacantTiles);
                    if (!grid.TryGetTileAt(occupiableTile, out var tile))
                    {
                        Debug.LogError($"Failed to get tile at position {occupiableTile}.");
                    }

                    if (!grid.TryOccupy(entity.GetComponent<Occupant>(), tile))
                    {
                        Debug.LogError($"Failed to occupy tile at position {occupiableTile} with entity {entity.name}.");
                    }
                }
            }
        }

        Vector2Int GetRandomVacantTile(List<Vector2Int> vacantTiles)
        {
            if (vacantTiles.Count == 0)
            {
                Debug.LogError("No vacant tiles available.");
                return Vector2Int.zero;
            }

            int randomIndex = Random.Range(0, vacantTiles.Count);
            var selectedTile = vacantTiles[randomIndex];
            vacantTiles.RemoveAt(randomIndex);
            return selectedTile;
        }
    }
}
