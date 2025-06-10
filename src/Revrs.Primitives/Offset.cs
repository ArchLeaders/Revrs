using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Revrs.Primitives;

[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 4)]
public readonly unsafe struct Offset<T> : IStructReverser where T : unmanaged
{
    private readonly uint _offset;

    public Offset()
    {
    }
    
    public Offset(uint offset)
    {
        _offset = offset;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get<TParent>(ref TParent relativeOffset) where TParent : unmanaged
    {
        fixed (TParent* loc = &relativeOffset) {
            byte* ptr = (byte*)loc + _offset;
            return ref *(T*)ptr;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpan<TParent>(ref TParent relativeOffset, int length) where TParent : unmanaged
    {
        fixed (TParent* loc = &relativeOffset) {
            byte* ptr = (byte*)loc + _offset;
            return new Span<T>(ptr, length);
        }
    }

    public static void Reverse(in Span<byte> slice)
    {
        slice[..4].Reverse();
    }
}