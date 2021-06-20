using System.Diagnostics;

namespace PavEcsGame.Components
{
    [DebuggerDisplay("{Source} -> {Target}")]
    public struct CollisionEvent<T> where T : struct, IEntity
    {
        public T Source;

        public T Target;

        //public NewPositionComponent Position;
    }
}