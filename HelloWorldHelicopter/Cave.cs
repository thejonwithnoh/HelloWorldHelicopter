using HelloWorldHelicopter.Properties;
using HelloWorldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloWorldHelicopter
{
    public class Cave
    {
        private Random Generator { get; set; }
        private LinkedList<Slice> Slices { get; set; }

        public Cave()
        {
            Generator = new Random();
            Slices = new LinkedList<Slice>();
            int ceiling = (Console.BufferHeight - Settings.Default.CaveHeight) / 2;
            for (int x = 0; x < Console.BufferWidth; x ++)
            {
                if (x < Console.BufferWidth / 4)
                {
                    Slices.AddLast(new Slice(ceiling, ceiling + Settings.Default.CaveHeight + 1));
                }
                else
                {
                    AddSlice();
                }
            }
        }

        public bool Hit(Helicopter helicopter)
        {
            return Slices
                .Select((slice, index) => new { Slice = slice, Index = index })
                .Any(indexedSlice => indexedSlice.Slice.Hit(helicopter, indexedSlice.Index));
        }

        public void Generate()
        {
            Slices.RemoveFirst();
            AddSlice();
        }

        private void AddSlice()
        {
            Slices.AddLast
            (
                new Slice
                (
                    Slices.Last.Value,
                    Generator.NextDouble() < Settings.Default.LevelProbability ?
                        0 :
                        Generator.NextDouble() < 0.5 ?
                            -1 :
                            1
                )
            );
        }

        public void Write(ConsoleBuffer buffer)
        {
            int x = 0;
            foreach (var caveSlice in Slices)
            {
                caveSlice.Write(buffer, x++);
            }
        }

        private class Slice
        {
            private const string HelloWorld = "Hello, world! ";

            public int Ceiling { get; private set; }
            public int Floor { get; private set; }

            public int Height
            {
                get { return Floor - Ceiling - 1; }
            }

            public Slice(int ceiling, int floor)
            {
                Ceiling = ceiling;
                Floor = floor;
            }

            public Slice(Slice previous, int delta)
            {
                delta = delta > 0 ?
                    Math.Min(delta, Console.BufferHeight - previous.Floor - 1) :
                    Math.Max(delta, -previous.Ceiling);

                Ceiling = previous.Ceiling + delta;
                Floor = previous.Floor + delta;
            }

            public bool Hit(Helicopter helicopter, int x)
            {
                return helicopter.Parts.Any(part => part.ContainsX(x) && (part.Y <= Ceiling || part.Y >= Floor));
            }

            public void Write(ConsoleBuffer buffer, int x)
            {
                for (int y = Ceiling, i = 0; y >= 0; y--, i++)
                {
                    buffer.Write(x, y, 0x20, HelloWorld[i % HelloWorld.Length]);
                }
                for (int y = Ceiling + 1; y <= Floor - 1; y++)
                {
                    buffer.Write(x, y, 0x00, ' ');
                }
                for (int y = Floor, i = 0; y < Console.BufferHeight; y++, i++)
                {
                    buffer.Write(x, y, 0x20, HelloWorld[i % HelloWorld.Length]);
                }
            }
        }
    }
}