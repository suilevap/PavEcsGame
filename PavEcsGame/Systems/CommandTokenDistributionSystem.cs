using System;
using Leopotam.Ecs;
using PavEcsGame.Components;
using PavEcsGame.Extensions;
using PavEcsGame.Systems.Managers;

namespace PavEcsGame.Systems
{
    public class CommandTokenDistributionSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly TimeSpan _autoRechargeTime;
        private EcsFilter<WaitCommandTokenComponent> _waitTokenFilter;

        private EcsFilter<CommandTokenComponent> _withTokenFilter;
        private DateTime _previousRecharge;

        public CommandTokenDistributionSystem(TimeSpan autoRechargeTime)
        {
            _autoRechargeTime = autoRechargeTime;   
        }
        public void Init()
        {
            _previousRecharge = DateTime.UtcNow;
        }

        public void Run()
        {
            //cleanup
            foreach (var i in _withTokenFilter)
            {
                if (_withTokenFilter.Get1(i).ActionCount <= 0)
                {
                    ref var ent = ref _withTokenFilter.GetEntity(i);
                    ent.Del<CommandTokenComponent>();
                }
            }
            //recharge
            if (_withTokenFilter.IsEmpty() || (DateTime.UtcNow - _previousRecharge) > _autoRechargeTime)
            {
                _previousRecharge = DateTime.UtcNow;
                foreach(var i in _waitTokenFilter)
                {
                    var ent = _waitTokenFilter.GetEntity(i);
                    ent.Get<CommandTokenComponent>() = _waitTokenFilter.Get1(i).RechargeValue;
                }
            }
        }
    }
}