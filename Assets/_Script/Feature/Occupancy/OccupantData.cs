using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    [CreateAssetMenu(menuName = "Occupancy/Occupant Data")]
    public class OccupantData : ScriptableObject
    {
        public int Width = 1;
        public int Height = 1;
        
        [Space(10)]
        public EntityTraitFlags Traits;
    }

}