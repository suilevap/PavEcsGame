using System;
using System.Text;
using Leopotam.Ecs;
using Leopotam.Ecs.Types;
using PavEcsGame.Components;
using PavEcsGame.GameLoop;

namespace PavEcsGame.Systems
{
    class SymbolReRenderAllSystem : IEcsInitSystem, IEcsRunSystem
    {
        private IMapData<PositionComponent, EcsEntity> _map;
        private StringBuilder _sb;

        public void Init()
        {
            _sb = new StringBuilder(_map.MaxPos.Value.X * _map.MaxPos.Value.Y);
        }

        public void Run()
        {

            _sb.Clear();
            for (int y = _map.MinPos.Value.Y; y < _map.MaxPos.Value.Y; y++)
            {
                for (int x = _map.MinPos.Value.X; x < _map.MaxPos.Value.X; x++)
                {
                    var pos = new PositionComponent(new Int2(x, y));
                    ref var ent = ref _map.Get(pos);
                    Console.SetCursorPosition(x, y);
                    char symbol;
                    if (!ent.IsNull() && ent.Has<SymbolComponent>())
                    {
                        ref var ch = ref ent.Get<SymbolComponent>();
                        symbol = ch.Value;
                    }
                    else
                    {
                        symbol = ' ';
                    }
                    _sb.Append(symbol);
                }
                _sb.AppendLine();
            }

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.Write(_sb.ToString());
        }
        //public void Run()
        //{
        //    Console.CursorVisible = false;


        //    for (int y = _map.MinPos.Y; y < _map.MaxPos.Y; y++)
        //    {
        //        //Console.SetCursorPosition(0, y);
        //        Console.CursorTop = y;
        //        for (int x = _map.MinPos.X; x < _map.MaxPos.X; x++)
        //        {
        //            var pos = new Int2(x, y);
        //            ref var ent = ref _map.Get(pos);
        //            Console.SetCursorPosition(pos.X, pos.Y);
        //            char symbol;
        //            if (!ent.IsNull() && ent.Has<SymbolComponent>())
        //            {
        //                ref var ch = ref ent.Get<SymbolComponent>();
        //                symbol = ch.Value;
        //            }
        //            else
        //            {
        //                symbol = ' ';
        //            }
        //            Console.Write(symbol);
        //        }
        //    }
        //}
    }
}
