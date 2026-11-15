using Alchemy.Inspector;
using Gimersia.Feature.Occupancy;
using UnityEngine;

namespace Gimersia.Gameplay
{
    public class Entity : MonoBehaviour
    {
        [Required] public EntityData Data;
        public Occupant Occupant { get; private set; }      // runtime representation

        void Start()
        {
            Occupant = new Occupant(Data.occupancyData);
        }
        void OnValidate()
        {

            if (Data == null)
            {
                Debug.LogError($"{name} requires EntityData.", this);
                return;
            }

            if (Data.occupancyData == null)
            {
                Debug.LogError($"{name}'s EntityData requires OccupantData.", this);
                return;
            }
        }
    }
}
