namespace PavEcsGame.Components
{
    public struct AreaResultComponent<T>
    {
        public byte Revision;
        public IMapData<PositionComponent, T> Data;
    }
}
