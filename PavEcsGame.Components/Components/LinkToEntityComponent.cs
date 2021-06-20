

namespace PavEcsGame.Components
{
    public struct LinkToEntityComponent<T> where T : IEntity
    {
        public T TargetEntity;
    }
}
