using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using PavEcsGame.Components;

namespace PavEcsGame
{
    public readonly struct EcsEntity : IEntity, IEquatable<EcsEntity>
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
            Leopotam.EcsLite.EcsEntityExtensions.EqualsTo(ent1, ent2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in EcsEntity ent1, in EcsEntity ent2) =>
            !Leopotam.EcsLite.EcsEntityExtensions.EqualsTo(ent1, ent2);

        public override string ToString() => _ent.ToString() ?? "<empty>";

        public bool Equals(EcsEntity other)
        {
            return _ent.Equals(other._ent);
        }

        public override bool Equals(object? obj)
        {
            return obj is EcsEntity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _ent.GetHashCode();
        }

    }
}