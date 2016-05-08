using HelloWorldHelicopter.Properties;
using HelloWorldLibrary;
using System;
using System.Threading;

namespace HelloWorldHelicopter
{
    public class Game
    {
        private ConsoleBuffer buffer;
        private Helicopter helicopter;
        private Cave cave;

        public Game()
        {
            Console.Title = "Hello, world! Helicopter";
            Console.BufferHeight = Console.WindowHeight = Settings.Default.WindowHeight.Clamp(0, Console.LargestWindowHeight);
            Console.BufferWidth = Console.WindowWidth = Settings.Default.WindowWidth.Clamp(0, Console.LargestWindowWidth);
            Console.CursorVisible = false;
            
            buffer = new ConsoleBuffer();
        }

        public void Run()
        {
            while (true)
            {
                int distance = 0;
                helicopter = new Helicopter(3, Console.BufferHeight / 2);
                cave = new Cave();

                Display(buffer);
                buffer.WriteCenter("Ready");
                buffer.Display();

                Utility.WaitForKey(ConsoleKey.Spacebar);

                while (!cave.Hit(helicopter))
                {
                    DateTime timestamp = DateTime.Now;
                    helicopter.Fly(Utility.IsKeyDown(ConsoleKey.Spacebar));

                    if (Utility.IsKeyDown(ConsoleKey.Enter))
                    {
                        buffer.WriteCenter("Paused");
                        buffer.Display();
                        Utility.WaitForKey(ConsoleKey.Spacebar);
                    }

                    distance++;
                    cave.Generate();
                    Display(buffer);
                    Thread.Sleep(Math.Max(0, (timestamp + TimeSpan.FromMilliseconds(1000 / Settings.Default.FPS) - DateTime.Now).Milliseconds));
                }

                buffer.WriteCenter("Hello, wall...", "Distance: " + distance);
                buffer.Display();
                Utility.WaitForKey(ConsoleKey.Enter);
            }
        }

        private void Display(ConsoleBuffer buffer)
        {
            cave.Write(buffer);
            helicopter.Write(buffer);
            buffer.Display();
        }

        public static void Main(string[] args)
        {
            new Game().Run();
        }
    }
}