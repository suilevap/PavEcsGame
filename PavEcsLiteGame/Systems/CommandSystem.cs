using System.IO;
using System.Text;
using DeBroglie;
using DeBroglie.Models;
using DeBroglie.Topo;
using Leopotam.EcsLite;
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
        public void Run(EcsSystems systems)
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
            var lines = await File.ReadAllLinesAsync(fileName);

            if (lines == null || lines.Length == 0)
                return;
            CreateMapDataComponent(fileName, lines);
        }

        public async void GenerateMap(string fileName, string pattern)
        {
            var lines = await File.ReadAllLinesAsync(fileName);

            if (lines == null || lines.Length == 0)
                return;
            var width = lines[0].Length;
            var height = lines.Length;
            // Define some sample data
            ITopoArray<char> sample = TopoArray.Create(new[]
            {
                new[]{ 'x', 'x', 'x'},
                new[]{ '.', '.', 'x'},
                new[]{ '.', '.', '.'},
            }, periodic: false);
            
            // Specify the model used for generation
            var model = new AdjacentModel(sample.ToTiles());
            // Set the output dimensions
            var topology = new GridTopology(width, height, periodic: false);
            // Acturally run the algorithm
            var propagator = new TilePropagator(model, topology);
            var status = propagator.Run();
            if (status != Resolution.Decided) throw new InvalidDataException("Undecided");
            var output = propagator.ToValueArray<char>();
            // Display the results

            string[] outputLines = new string[height];
            for (var y = 0; y < height; y++)
            {
                StringBuilder sb = new StringBuilder(width);

                for (var x = 0; x < width; x++)
                {
                    var item = output.Get(x, y);
                    var defined = lines[y][x];
                    if (defined != '.')
                    {
                        item = defined;
                    }
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