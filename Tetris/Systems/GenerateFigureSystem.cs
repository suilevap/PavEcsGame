using Leopotam.EcsLite;
using PavEcsGame.Components;
using PavEcsSpec.EcsLite;
using Tetris.Components;

namespace Tetris.Systems
{
    internal class GenerateFigureSystem : IEcsRunSystem, IEcsSystemSpec
    {
        private readonly EcsFilterSpec.Inc<EcsReadonlySpec<Figure>> _figureSpec;

        private readonly EcsFilterSpec
            .Inc<EcsReadonlySpec<MapComponent>> _mapSpec;

        private readonly EcsEntityFactorySpec<EcsSpec<Figure, PositionComponent>> _figureFactory;

        //private readonly int[] _shapes = new[]
        //{
        //    0b0000_0110_0110_0000, //O
        //    0b0100_0100_0100_0100, //I
        //    0b0000_0011_0110_0000, //S
        //    0b0000_0110_0011_0000, //Z
        //    0b0100_0100_0110_0000, //L
        //    0b0010_0010_0110_0000, //J
        //    0b0000_0111_0010_0000, //T
        //    //0123_4567_89AB_CDEF
        //    //C840_D951_EA62_FB73
        //    //1000_3000_4000_4000
        //    //4321_
        //    //(x & 1<<C) >> 0 | (x & 1<<8)>> 1 |
        //};

        //private readonly int[][] _shapesRotation = new[]
        //{
        //    new[]
        //    {
        //        0b0000_0110_0110_0000, //O
        //    },
        //    new[]
        //    {
        //        0b0100_0100_0100_0100, //I
        //        0b0000_1111_0000_0000,
        //        0b0010_0010_0010_0010,
        //        0b0000_0000_1111_0000,
        //    },
        //    new[]
        //    {
        //        //0000
        //        //0011
        //        //0110
        //        //0000, //S

        //        //0100
        //        //0110
        //        //0010
        //        //0000, //S

        //        0b0000_0011_0110_0000, //S
        //        0b0100_0110_0010_0000, //S

        //    },
        //    new[]
        //    {
        //        0b0000_0110_0011_0000, //Z
        //        0b0010_0110_0100_0000,
        //    },
        //    new []
        //    {
        //        //0100
        //        //0100
        //        //0110
        //        //0000

        //        //0000
        //        //1110
        //        //1000
        //        //0000

        //        //0110
        //        //0010
        //        //0010
        //        //0000

        //        //0000
        //        //0010
        //        //1110
        //        //0000
        //        0b0100_0100_0110_0000, //L
        //        0b0000_1110_1000_0000, //L
        //        0b0110_0010_0010_000, //L
        //        0b0000_0010_1110_000, //L
        //    },
        //    new []
        //    {
        //        0b0010_0010_0110_0000, //J
        //        0b0000_1000_1110_0000,
        //        0b0110_0100_0100_000,
        //        0b0000_1110_0010_000
        //    },
        //    new []
        //    {
        //        //0000
        //        //1110
        //        //0100
        //        //0000

        //        //0100
        //        //1100
        //        //0100
        //        //0000
        //        0b0000_1110_0100_0000, //T
        //        0b0100_1100_0100_0000, //T
        //        0b0100_1110_0000_0000, //T
        //        0b0100_0110_0100_0000, //T

        //    }

        //};

        private readonly Random _rnd;

        public GenerateFigureSystem(EcsUniverse universe, int rndSeed)
        {
            _rnd = new Random(rndSeed);
            universe.Register(this)
                .Build(ref _figureSpec)
                .Build(ref _mapSpec)
                .Build(ref _figureFactory);
        }

        public void Run(IEcsSystems systems)
        {
            if (_figureSpec.Filter.GetEntitiesCount() == 0)
            {
                var mapPool = _mapSpec.Include.Pool1;
                int xOffset = 0;
                foreach (EcsUnsafeEntity ent in _mapSpec.Filter)
                {
                    ref readonly var map = ref mapPool.Get(ent);
                    xOffset = map.Width / 2 - 2;
                    if (xOffset > 0)
                    {
                        break;
                    }
                }

                _figureFactory.NewUnsafeEntity()
                    .Add(_figureFactory.Pools,
                        new Figure()
                        {
                            Shape = FiguresData.Instance.GetRandomFigureType(_rnd)
                        },
                        new PositionComponent(xOffset,1));
            }
        }
    }
}
