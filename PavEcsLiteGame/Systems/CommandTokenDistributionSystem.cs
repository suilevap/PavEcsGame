using System;
using Leopotam.Ecs;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    public partial class CommandTokenDistributionSystem : IEcsRunSystem, IEcsInitSystem, IEcsSystemSpec
    {
        private readonly TimeSpan _autoRechargeTime;
        private DateTime _previousRecharge;


        [Entity]
        private partial struct WaitTokenEnt
        {
            public partial ref readonly WaitCommandTokenComponent WaitToken();
            public partial OptionalComponent<CommandTokenComponent> CommandToken();
        }

        [Entity]
        private partial struct WithTokenEnt
        {
            public partial RequiredComponent<CommandTokenComponent> CommandToken();
        }

        public CommandTokenDistributionSystem(TimeSpan autoRechargeTime, EcsSystems universe)
            : this (universe)
        {
            _autoRechargeTime = autoRechargeTime;
        }
        public void Init(EcsSystems systems)
        {
            _previousRecharge = DateTime.UtcNow;
        }

        public void Run(EcsSystems systems)
        {
            CleanupEmptyCommandTokens();
            if (_providers.WithTokenEntProvider.Filter.IsEmpty() || (DateTime.UtcNow - _previousRecharge) > _autoRechargeTime)
            {
                RechargeCommandTokens();
            }

            void CleanupEmptyCommandTokens()
            {
                foreach (var ent in _providers.WithTokenEntProvider)
                {
                    if (ent.CommandToken().Get().ActionCount <= 0)
                    {
                        ent.CommandToken().Remove();
                    }
                }
            }

            void RechargeCommandTokens()
            {
                _previousRecharge = DateTime.UtcNow;

                foreach (var ent in _providers.WaitTokenEntProvider)
                {
                    ent.CommandToken().Ensure() = ent.WaitToken().RechargeValue;
                }
            }
        }
    }
}