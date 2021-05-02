using Leopotam.Ecs;

namespace PavEcsGame.Components
{
    public readonly struct WaitCommandTokenComponent 
    {
        public readonly CommandTokenComponent RechargeValue;

        public WaitCommandTokenComponent(int rechargeValue) 
        {
            RechargeValue = new CommandTokenComponent()
            {
                ActionCount = rechargeValue
            };
        }
    }
}