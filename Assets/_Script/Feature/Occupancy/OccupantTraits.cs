using System;

namespace Gimersia.Feature.Occupancy
{
    [Flags]
    public enum EntityTraitFlags
    {
        None = 0,

        Carnivore = 1 << 2,
        Herbivore = 1 << 3,
        Omnivore = 1 << 4,

        Hungry = 1 << 5,
        Thirsty = 1 << 6,
    }

}