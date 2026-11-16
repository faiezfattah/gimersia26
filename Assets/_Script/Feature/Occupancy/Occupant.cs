using UnityEngine;
using Alchemy.Inspector;
using Gimersia.Gameplay;
using System.Collections.Generic;

namespace Gimersia.Feature.Occupancy
{
    public sealed class Occupant : MonoBehaviour
    {
        [Required] public OccupantData Data;
        public List<Rule> Rules;
    }
}
