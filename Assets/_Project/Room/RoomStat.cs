using Unity.Entities;

namespace Project
{
    public struct RoomStat : IBufferElementData
    {
        public PanelType Type;
        public int Count;
        public double Price;
    }
}
