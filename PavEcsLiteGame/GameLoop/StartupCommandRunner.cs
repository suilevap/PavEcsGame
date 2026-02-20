using PavEcsGame.Systems;

namespace PavEcsGame.GameLoop
{
    internal static class StartupCommandRunner
    {
        private const string DefaultMapPath = "Data/lightTest.txt";

        public static void Run(string[] args, CommandSystem cmdSystem)
        {
            if (args.Length == 0)
            {
                cmdSystem.LoadMap(DefaultMapPath);
                return;
            }

            var i = 0;
            while (i < args.Length)
            {
                switch (args[i])
                {
                    case "-m":
                        cmdSystem.LoadMap(args[++i]);
                        break;
                    case "-g":
                        cmdSystem.GenerateMap(args[++i], args[++i]);
                        break;
                }
                i++;
            }
        }
    }
}
