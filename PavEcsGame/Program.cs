using PavEcsGame.GameLoop;
using System;
using System.Threading;

namespace LeoEcs_test
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
                //Thread.Sleep(16);
                //await Task.Delay(TimeSpan.FromSeconds(1));
            }
            //Console.WriteLine("Hello World!");
        }
    }
}
