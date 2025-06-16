using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Revrs.Primitives;

[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 4)]
public readonly unsafe struct Offset<T> : IStructReverser where T : unmanaged
{
    public readonly uint Value;

    public Offset()
    {
    }
    
    public Offset(uint value)
    {
        Value = value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get<TParent>(in TParent relativeOffset) where TParent : unmanaged
    {
        if (Value <= 0) {
            return ref Unsafe.NullRef<T>();
        }
        
        fixed (TParent* loc = &relativeOffset) {
            byte* ptr = (byte*)loc + Value;
            return ref *(T*)ptr;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> GetSpan<TParent>(in TParent relativeOffset, int length) where TParent : unmanaged
    {
        if (Value <= 0) {
            return [];
        }
        
        fixed (TParent* loc = &relativeOffset) {
            byte* ptr = (byte*)loc + Value;
            return new Span<T>(ptr, length);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> GetStr<TParent>(in TParent relativeOffset) where TParent : unmanaged
    {
        if (Value <= 0) {
            return [];
        }
        
        fixed (TParent* loc = &relativeOffset) {
            byte* ptr = (byte*)loc + Value;
            
            int length = -1;
            while (ptr[++length] is not 0) { }
            
            return new Span<byte>(ptr, length);
        }
    }

    public static void Reverse(in Span<byte> slice)
    {
        slice[..4].Reverse();
    }
}