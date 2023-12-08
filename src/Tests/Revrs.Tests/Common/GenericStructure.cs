using System.Runtime.InteropServices;

namespace Revrs.Tests.Common;

[StructLayout(LayoutKind.Explicit, Pack = 2, Size = 18)]
public struct GenericStructure
{
    [FieldOffset(0)]
    public Endianness Endianness = Endianness.Big;

    [FieldOffset(2)]
    public byte UInt8 = byte.MaxValue;

    // 1 byte of padding (should be accounted for)

    [FieldOffset(4)]
    public ushort UInt16 = ushort.MaxValue;

    [FieldOffset(6)]
    public uint UInt32 = uint.MaxValue;

    [FieldOffset(10)]
    public ulong UInt64 = ulong.MaxValue;

    public GenericStructure()
    {

    }

    public class Reverser : IStructReverser
    {
        public static void Reverse(in Span<byte> slice)
        {
            slice[04..06].Reverse();
            slice[06..10].Reverse();
            slice[10..18].Reverse();
        }
    }
}
