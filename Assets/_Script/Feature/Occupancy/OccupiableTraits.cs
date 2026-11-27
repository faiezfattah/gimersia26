using System;

namespace Gimersia.Feature.Occupancy
{
    [Flags]
    public enum TileTraitFlags
    {
        None = 0,
        Land = 1 << 0,
        Water = 1 << 1,
        Shaded = 1 << 2,

        Sitting = 1 << 3,
        Standing = 1 << 4,
        NearWindow = 1 << 5,
        DriverSeat = 1 << 6,
        NavigatorSeat = 1 << 7,
        BackRow = 1 << 8,
        FrontRow = 1 << 9,
    }
}
