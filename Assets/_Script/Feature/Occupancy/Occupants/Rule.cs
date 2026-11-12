using UnityEngine;

namespace Gimersia.Feature.Occupancy
{
    [CreateAssetMenu(menuName = "Occupancy/Neighbor Rule")]
    public class Rule : ScriptableObject
    {
        public string RuleId;
        public Subject Subject;
        public Scope Scope;
        [Min(1)] public int Extent = 1;
        public Polarity Polarity;   // Avoid / Require
        public TraitFlags Target;
    }
    public enum Subject
    {
        Entity,
        Tile,
    }
    public enum Scope
    {
        Self,
        Adjacent,
        Near,
        Row,
        Column
    }

    public enum Polarity 
    {
        Avoid,
        Require
    }
}