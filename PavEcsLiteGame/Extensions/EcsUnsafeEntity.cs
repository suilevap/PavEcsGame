using System;
using System.Collections.Generic;
using System.Text;

namespace PavEcsGame.Extensions
{
    public readonly struct EcsUnsafeEntity
    {
        public readonly int Id;

        public EcsUnsafeEntity(int id)
        {
            Id = id;
        }

        public static implicit operator int(in EcsUnsafeEntity ent) => ent.Id;
        public static explicit operator EcsUnsafeEntity(int id) => new EcsUnsafeEntity(id);
    }

}
