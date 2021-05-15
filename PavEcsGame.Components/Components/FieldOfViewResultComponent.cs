using Leopotam.Ecs.Types;

namespace PavEcsGame.Components
{
    public struct FieldOfViewResultComponent
    {
        public Int2 Center;
        public int Radius;
        public IMapData<Int2, float> Data;
    }
}
