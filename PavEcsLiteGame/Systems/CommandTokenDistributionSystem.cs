using System;
using Leopotam.Ecs;
using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsGame.Systems.Managers;
using PavEcsSpec.EcsLite;

namespace PavEcsGame.Systems
{
    public class CommandTokenDistributionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly TimeSpan _autoRechargeTime;
        private DateTime _previousRecharge;
        private readonly EcsFilterSpec<EcsSpec<WaitCommandTokenComponent>, EcsSpec<CommandTokenComponent>, EcsSpec> _waitTokenSpec;
        private readonly EcsFilterSpec<EcsSpec<CommandTokenComponent>, EcsSpec, EcsSpec> _withTokenSpec;

        public CommandTokenDistributionSystem(TimeSpan autoRechargeTime, EcsUniverse universe)
        {
            _autoRechargeTime = autoRechargeTime;
            _waitTokenSpec = universe.CreateFilterSpec(
                EcsSpec<WaitCommandTokenComponent>.Build(),
                EcsSpec<CommandTokenComponent>.Build(),
                EcsSpec.Empty()
            );

            _withTokenSpec = universe.CreateFilterSpec(
                EcsSpec<CommandTokenComponent>.Build(),
                EcsSpec.Empty(),
                EcsSpec.Empty()
            );
        }
        public void Init(EcsSystems systems)
        {
            _previousRecharge = DateTime.UtcNow;
        }

        public void Run(EcsSystems systems)
        {
            CleanupEmptyCommandTokens();
            if (_withTokenSpec.Filter.IsEmpty() || (DateTime.UtcNow - _previousRecharge) > _autoRechargeTime)
            {
                RechargeCommandTokens();
            }

            void CleanupEmptyCommandTokens()
            {
                var tokenPool = _withTokenSpec.Include.Pool1;
                foreach (EcsUnsafeEntity ent in _withTokenSpec.Filter)
                {
                    if (tokenPool.Get(ent).ActionCount <= 0)
                    {
                        tokenPool.Del(ent);
                    }
                }
            }

            void RechargeCommandTokens()
            {
                _previousRecharge = DateTime.UtcNow;
                EcsPool<CommandTokenComponent> tokenPool = _waitTokenSpec.Optional.Pool1;
                EcsPool<WaitCommandTokenComponent> waitTokenPool = _waitTokenSpec.Include.Pool1;

                foreach (EcsUnsafeEntity ent in _waitTokenSpec.Filter)
                {
                    tokenPool.Set(ent) = waitTokenPool.Get(ent).RechargeValue;
                }
            }
        }
    }
}