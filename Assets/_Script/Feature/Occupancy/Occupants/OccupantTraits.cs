using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    [CreateAssetMenu(menuName = "Occupancy/Occupant Data")]
    public class OccupantData : ScriptableObject
    {
        public Sprite Icon;
        public int Width = 1;
        public int Height = 1;
        
        [Space(10)]
        public TraitFlags Traits;          // we show this as icons
        public List<Rule> Rules;            // we show this as dialogue
    }
    
    [Flags]
    public enum TraitFlags
    {
        None = 0,
        Land = 1 << 0,
        Water = 1 << 1,

        Carnivore = 1 << 2,
        Herbivore = 1 << 3,
        Omnivore = 1 << 4,

        Hungry = 1 << 5,
        Thirsty = 1 << 6,
    }

}