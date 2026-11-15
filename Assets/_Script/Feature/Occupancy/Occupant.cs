namespace Gimersia.Feature.Occupancy
{
    public sealed class Occupant
    {
        public OccupantData Data { get; }

        public Occupant(OccupantData data)
        {
            Data = data;
        }
    }
}
