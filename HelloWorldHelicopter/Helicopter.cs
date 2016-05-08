using HelloWorldHelicopter.Properties;
using HelloWorldLibrary;
using System;
using System.Collections.Generic;

namespace HelloWorldHelicopter
{
    public class Helicopter
    {
        public int RotorAngle { get; private set; }
        public IEnumerable<Part> Parts { get; private set; }

        private double x;
        private double y;
        private double dy;

        public int X { get { return (int)x; } }
        public int Y { get { return (int)y; } }

        public Helicopter()
        {
            var parts = new HashSet<Part>();
            parts.Add(new Body(this));
            parts.Add(new MainRotor(this));
            parts.Add(new TailRotor(this));
            Parts = parts;
        }

        public Helicopter(int x, int y) : this()
        {
            this.x = x;
            this.y = y;
        }

        public void Fly(bool gas)
        {
            RotorAngle = (RotorAngle + 45) % 180;

            dy += gas ? -Settings.Default.Gravity : Settings.Default.Gravity;
            dy = dy.Clamp(-Settings.Default.TerminalVelocity, Settings.Default.TerminalVelocity);

            y += dy;
            y = y.Clamp(0, Console.BufferHeight - 1);
        }

        public void Write(ConsoleBuffer buffer)
        {
            foreach (Part part in Parts)
            {
                part.Write(buffer);
            }
        }

        public abstract class Part
        {
            public Part(Helicopter owner)
            {
                Owner = owner;
            }

            public Helicopter Owner { get; private set; }

            public abstract int X { get; }
            public abstract int Y { get; }

            public int Width { get { return Display.Length; } }
            public int Height { get { return 1; } }

            public int XMax { get { return X + Width - 1; } }
            public int YMax { get { return Y + Height - 1; } }

            public abstract string Display { get; }

            public bool ContainsX(int x)
            {
                return x.IsBetween(X, X + Width);
            }

            public bool ContainsY(int y)
            {
                return y.IsBetween(Y, Y + Height);
            }

            public void Write(ConsoleBuffer buffer)
            {
                buffer.Write(X, Y, Display);
            }
        }

        private class Body : Part
        {
            public Body(Helicopter owner) : base(owner) { }

            public override int X { get { return Owner.X + 1; } }
            public override int Y { get { return Owner.Y; } }

            public override string Display { get { return "-<O>"; } }
        }

        private class MainRotor : Part
        {
            public MainRotor(Helicopter owner) : base(owner) { }

            public override int X { get { return Owner.X + 3 - Display.Length / 2; } }
            public override int Y { get { return Owner.Y - 1; } }

            public override string Display
            {
                get
                {
                    switch (Owner.RotorAngle)
                    {
                        case 0: return "--+--";
                        case 45:
                        case 135: return "-+-";
                        case 90: return "+";
                        default: return null;
                    }
                }
            }
        }

        private class TailRotor : Part
        {
            public TailRotor(Helicopter owner) : base(owner) { }

            public override int X { get { return Owner.X; } }
            public override int Y { get { return Owner.Y; } }

            public override string Display
            {
                get
                {
                    switch (Owner.RotorAngle)
                    {
                        case 0:
                        case 90: return "*";
                        case 45:
                        case 135: return "+";
                        default: return null;
                    }
                }
            }
        }
    }
}