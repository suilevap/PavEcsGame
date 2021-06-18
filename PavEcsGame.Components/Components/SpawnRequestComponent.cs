using System;
using System.Collections.Generic;
using System.Text;
using Leopotam.Ecs;

namespace PavEcsGame.Components
{
    public struct SpawnRequestComponent
    {
        public EntityType Type;
    }

    public enum EntityType
    {
        Player,
        Wall,
        Enemy,
        Light,
        Acid,
        Electricity
    }
}
