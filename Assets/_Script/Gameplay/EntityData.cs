using Gimersia.Feature.Occupancy;
using UnityEngine;

namespace Gimersia.Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/Entity Data")]
    public class EntityData : ScriptableObject
    {
        public Sprite Icon;
        public OccupantData occupancyData;
        // public SpiteData spriteData; // for emotions
        // other things like voice, dialogue nodes, etc
    }
}
