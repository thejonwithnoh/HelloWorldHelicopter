using System.Runtime.InteropServices;

namespace HelloWorldLibrary
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CharInfo
    {
        public char Character;
        public short Attributes;
    }
}