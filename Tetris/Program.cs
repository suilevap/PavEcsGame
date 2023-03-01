GameMainContainer game = new GameMainContainer();

game.Start();
while (game.IsAlive)
{
    game.Update();
}