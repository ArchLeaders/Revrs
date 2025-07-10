using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace Revrs.Primitives;

/// <summary>
/// A view over any region of bytes as a string
/// </summary>
public readonly ref struct StringView
{
    public readonly ReadOnlySpan<byte> Value;

    public StringView()
    {
    }

    public StringView(ReadOnlySpan<byte> value)
    {
        Value = value;   
    }

    public StringView(in byte value)
    {
        Value = new ReadOnlySpan<byte>(in value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator StringView(Span<byte> utf8) => new(utf8);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator StringView(ReadOnlySpan<byte> utf8) => new(utf8);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe implicit operator StringView(byte* ptr) => new(in Unsafe.AsRef<byte>(ptr));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override unsafe string? ToString()
    {
        fixed (byte* ptr = Value) {
            return Utf8StringMarshaller.ConvertToManaged(ptr);
        }
    }
}