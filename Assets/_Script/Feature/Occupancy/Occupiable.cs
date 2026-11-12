using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    public class Occupiable : MonoBehaviour
    {
        public OccupiableType Type;   
    }
    public enum OccupiableType
    {
        Land,
        Water,
    }
}