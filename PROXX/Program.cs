using System;
using static System.Console;

namespace PROXX
{
    public class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {

                var length = View.DrawEnterFieldLength();
                var blackHolesCount = View.DrawEnterBlackHolesCount(length);
                var view = new View(length, blackHolesCount);

                var successfulMove = true;
                var gameIsWon = false;

                while (true)
                {
                    view.DrawPlayingField();

                    if (!successfulMove || gameIsWon)
                        break;

                    int x;
                    x = view.DrawEnterCoordinates(nameof(x));

                    int y;
                    y = view.DrawEnterCoordinates(nameof(y));

                    var coordinates = (x, y);

                    char userInput = View.DrawFlagOrOpen();
                    switch (userInput)
                    {
                        case 'f':
                            view.Game.Flag(coordinates);
                            continue;
                        case 'u':
                            view.Game.UnFlag(coordinates);
                            continue;
                        default:
                            break;

                    }

                    successfulMove = view.Game.MakeMove(coordinates);

                    gameIsWon = view.Game.IsWon();
                }

                var currentColor = ForegroundColor;
                if (!successfulMove)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("Sorry! You lost.");
                }
                else if (gameIsWon)
                {
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("Congratulations! You won!");
                }

                ForegroundColor = currentColor;
                WriteLine("Press any key to play one more time!");
                ReadLine();
            }
        }
    }
}