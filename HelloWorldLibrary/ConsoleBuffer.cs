using System;
using System.Runtime.InteropServices;

namespace HelloWorldLibrary
{
    public class ConsoleBuffer
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Vector
        {
            public short X;
            public short Y;

            public Vector(int x, int y)
            {
                X = (short)x;
                Y = (short)y;
            }

            public int Product
            {
                get { return X * Y; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rectangle
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;

            public Rectangle(int left, int top, int right, int bottom)
            {
                Left = (short)left;
                Top = (short)top;
                Right = (short)right;
                Bottom = (short)bottom;
            }

            public Rectangle(Vector location, Vector size)
                : this(location.X, location.Y, location.X + size.X - 1, location.Y + size.Y - 1) { }

            public int Width
            {
                get { return Right - Left + 1; }
            }

            public int Height
            {
                get { return Bottom - Top + 1; }
            }
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle
        (
            int nStdHandle
        );

        [DllImport("kernel32.dll")]
        private static extern bool WriteConsoleOutput
        (
            IntPtr hConsoleOutput,
            CharInfo[] lpBuffer,
            Vector dwBufferSize,
            Vector dwBufferCoord,
            ref Rectangle lpWriteRegion
        );

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleFont
        (
            IntPtr hOutput,
            int fontIndex
        );

        public const short DefaultAttribute = 0x0F;

        private static IntPtr consoleHandle = GetStdHandle((int)StdHandle.Output);

        private CharInfo[] buffer;
        private Vector size;
        private Vector location;
        private Rectangle bounds;

        public ConsoleBuffer()
        {
            UpdateBuffer();
        }
        
        private void UpdateBuffer()
        {
            location = new Vector(Console.WindowLeft, Console.WindowTop);
            size = new Vector(Console.BufferWidth, Console.BufferHeight);
            bounds = new Rectangle(location, size);
            buffer = new CharInfo[size.Product];
        }

        public bool SetConsoleFont(int fontIndex)
        {
            return SetConsoleFont(consoleHandle, fontIndex);
        }

        public void Display()
        {
            WriteConsoleOutput(consoleHandle, buffer, size, location, ref bounds);
        }

        public void WriteCenter(params string[] lines)
        {
            WriteCenter(DefaultAttribute, lines);
        }

        public void WriteCenter(short attr, params string[] lines)
        {

            WriteCenter((Console.BufferHeight - lines.Length) / 2, attr, lines);
        }

        public void WriteCenter(int y, params string[] lines)
        {
            WriteCenter(y, DefaultAttribute, lines);
        }

        public void WriteCenter(int y, short attr, params string[] lines)
        {
            foreach (string line in lines)
            {
                Write((Console.BufferWidth - line.Length) / 2, y++, attr, line);
            }
        }

        public void Write(int x, int y, params string[] lines)
        {
            Write(x, y, DefaultAttribute, lines);
        }

        public void Write(int x, int y, short attr, params string[] lines)
        {
            foreach (var line in lines)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    Write(x + i, y, attr, line[i]);
                }
            }
        }

        public void Write(int x, int y, char ch)
        {
            Write(x, y, DefaultAttribute, ch);
        }

        public void Write(int x, int y, short attr, char ch)
        {
            try
            {
                int index = y * size.X + x;
                buffer[index].Character = ch;
                buffer[index].Attributes = attr;
            }
            catch
            {
                UpdateBuffer();
            }
        }

        public void Write(int x, int y, CharInfo charInfo)
        {
            Write(x, y, charInfo.Attributes, charInfo.Character);
        }
    }
}