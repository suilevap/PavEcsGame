using System;
using PavEcsGame.GameLoop;

namespace PavEcsGame
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
