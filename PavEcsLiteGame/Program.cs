using System;
using PavEcsGame.GameLoop;

namespace PavEcsLiteGame
{
    class Program
    {
        static void Main(string[] args)
        {
            GameMainContainer game = new GameMainContainer();

            game.Start();
            while (game.IsAlive)
            {
                game.Update();
            }
        }
    }
}
