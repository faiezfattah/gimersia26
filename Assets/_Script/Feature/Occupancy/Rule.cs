using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gimersia.Feature.Occupancy
{
    [CreateAssetMenu(menuName = "Occupancy/Rule")]
    public class Rule : ScriptableObject
    {
        [FormerlySerializedAs("ErrorText")]
        public string ErrorDialogue;
        public string WantText;
        public Subject Subject;
        public Scope Scope;
        [Min(1)] public int Extent = 1;
        public Polarity Polarity;   // Avoid / Require
        public EntityTraitFlags TargetEntity;
        public TileTraitFlags TargetTile;

        public override string ToString()
        {
            return $"{Polarity} entity={TargetEntity} tile={TargetTile} in {Scope} (extent={Extent})";
        }
    }
    [Flags]
    public enum Subject
    {
        Entity = 1 << 0,
        Tile = 1 << 1,
    }
    public enum Scope
    {
        Self,
        Adjacent,
        Surrounding,
        Row,
        Column
    }

    public enum Polarity
    {
        Avoid,
        Require
    }
}
