using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using PavEcsGame.Components;

namespace PavEcsGame
{
    public readonly struct EcsEntity : IEntity
    {
        private readonly EcsPackedEntityWithWorld _ent;

        public EcsEntity(EcsPackedEntityWithWorld ent)
        {
            _ent = ent;
        }

        public static implicit operator EcsPackedEntityWithWorld(in EcsEntity ent)
        {
            return ent._ent;
        }

        public static implicit operator EcsEntity(in EcsPackedEntityWithWorld ent)
        {
            return new EcsEntity(ent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in EcsEntity ent1, in EcsEntity ent2) =>
            Leopotam.EcsLite.EcsEntityExtensions.Equals(ent1, ent2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in EcsEntity ent1, in EcsEntity ent2) =>
            !Leopotam.EcsLite.EcsEntityExtensions.Equals(ent1, ent2);

        public override string ToString() => _ent.ToString();


    }
}