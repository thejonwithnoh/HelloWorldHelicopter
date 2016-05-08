using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HelloWorldLibrary
{
    public static class Utility
    {
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int key);

        public static bool IsKeyDown(ConsoleKey key)
        {
            return (GetKeyState((int)key) & short.MinValue) != 0;
        }

        public static void WaitForKey(ConsoleKey key)
        {
            ClearKeyBuffer();
            while (Console.ReadKey(true).Key != key) { }
        }

        public static void ClearKeyBuffer()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }

        public static IEnumerable<string> ToLines(this string str)
        {
            using (var stringReader = new StringReader(str))
            {
                while (stringReader.Peek() != -1)
                {
                    yield return stringReader.ReadLine();
                }
            }
        }

        public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) < 0;
        }

        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
        }
    }
}