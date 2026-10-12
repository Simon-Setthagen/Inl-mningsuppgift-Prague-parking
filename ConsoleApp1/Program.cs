using System;

class Program
{
    static void Main()
    {
        int x = 10;
        int y = 10;
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            Console.SetCursorPosition(x, y);
            Console.Write("[ ]"); // Vår "karaktär" som flyttas

            // Läs tangenttryckning
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            // Flytta position beroende på piltangent
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (y > 0) y--;
                    break;
                case ConsoleKey.DownArrow:
                    if (y < Console.WindowHeight - 1) y++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (x > 0) x--;
                    break;
                case ConsoleKey.RightArrow:
                    if (x < Console.WindowWidth - 1) x++;
                    break;
                case ConsoleKey.Escape:
                    return; // Avsluta programmet
            }
        }
    }
}