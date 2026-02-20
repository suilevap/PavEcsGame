using System.IO;
using System.Linq;
using System.Text;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;
using Leopotam.EcsLite;
using PavEcsGame.Common.Utils;
using PavEcsGame.Components.Events;
using PavEcsSpec.Generated;

namespace PavEcsGame.Systems
{
    internal partial class CommandSystem : IEcsRunSystem
    {
        public struct CommandComponent
        {
            public string Command;
            public string[] Args;
        }

        [Entity]
        private readonly partial struct Ent
        {
            public partial RequiredComponent<CommandComponent> Command();
        }

        [Entity(SkipFilter = true)]
        private readonly partial struct MapDataEnt
        {
            public partial ref MapRawDataEvent Event();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in _providers.EntProvider)
            {
                ref var cmd = ref ent.Command().Get();

                switch (cmd.Command)
                {
                    case "load":
                    {
                        var fileName = cmd.Args[0];
                        LoadMap(fileName);
                    }
                        break;
                }

                ent.Command().Remove();
            }
        }

        public async void LoadMap(string fileName)
        {
            var lines = await File.ReadAllLinesAsync(FileHelper.ResolvePath(fileName));

            if (lines == null || lines.Length == 0)
                return;
            CreateMapDataComponent(fileName, lines);
        }

        public async void GenerateMap(string fileName, string patternFilename)
        {
            var lines = await File.ReadAllLinesAsync(FileHelper.ResolvePath(fileName));

            if (lines == null || lines.Length == 0)
                return;
            var width = lines[0].Length;
            var height = lines.Length;
            // Define some sample data
            var patternLines = (await File.ReadAllLinesAsync(FileHelper.ResolvePath(patternFilename)))
                .Select(x => x.ToArray())
                .ToArray();

            var sample = TopoArray.Create(patternLines, false);

            //ITopoArray<char> sample = TopoArray.Create(new[]
            //{
            //    new[]{ '.', '.', 'x','.'},
            //    new[]{ 'x', 'x', 'x','.'},
            //    new[]{ 'x', '.', 'x','x'},
            //    //fnew[]{ 'x', '.', '.','.'},

            //}, periodic: false);

            // Specify the model used for generation
            var tiles = sample.ToTiles();
            //var model = new AdjacentModel(tiles);
            var model = new OverlappingModel(2);
            model.AddSample(tiles);

            // Set the output dimensions
            var topology = new GridTopology(width, height, false);
            // Acturally run the algorithm
            var propagator = new TilePropagator(model, topology);
            var status = propagator.Run();
            var iterations = 0;
            while (iterations++ < 100 && status != Resolution.Decided) status = propagator.Run();

            if (status != Resolution.Decided) throw new InvalidDataException("Undecided");
            var output = propagator.ToValueArray<char>();
            // Display the results

            var outputLines = new string[height];
            for (var y = 0; y < height; y++)
            {
                var sb = new StringBuilder(width);

                for (var x = 0; x < width; x++)
                {
                    var item = output.Get(x, y);
                    var defined = lines[y][x];
                    if (defined != '.') item = defined;
                    sb.Append(item);
                }

                outputLines[y] = sb.ToString();
            }

            CreateMapDataComponent(fileName, outputLines);
        }

        private void CreateMapDataComponent(string fileName, string[] lines)
        {
            ref var data = ref _providers.MapDataEntProvider.New().Event();
            data.Name = fileName;
            data.Data = lines;
        }
    }
}