using PavEcsGame.GameLoop;

namespace PavEcsGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var game = new GameMainContainer(args);

            game.Start();
            while (game.IsAlive) game.Update();
        }
    }
}