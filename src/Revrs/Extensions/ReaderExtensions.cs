using System.Runtime.InteropServices;

namespace Revrs.Extensions;

public static class ReaderExtensions
{
    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="slice"/> in the system endianness.
    /// </summary>
    /// <typeparam name="T">The primitive or struct type to read.</typeparam>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Read<T>(this Span<byte> slice) where T : struct
    {
        return ref MemoryMarshal.Cast<byte, T>(slice)[0];
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="slice"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read.</typeparam>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Read<T>(this Span<byte> slice, Endianness endianness) where T : unmanaged
    {
        if (slice.Length > 1 && endianness.IsNotSystemEndianness()) {
            slice.Reverse();
        }

        return ref MemoryMarshal.Cast<byte, T>(slice)[0];
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="slice"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <typeparamref name="R"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="R">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Read<T, R>(this Span<byte> slice, Endianness endianness) where T : unmanaged where R : IStructReverser
    {
        if (slice.Length > 1 && endianness.IsNotSystemEndianness()) {
            R.Reverse(slice);
        }

        return ref MemoryMarshal.Cast<byte, T>(slice)[0];
    }

    /// <summary>
    /// Read <typeparamref name="T"/>'s from the provided <paramref name="slice"/> until the end of the slice in the system endianness.
    /// </summary>
    /// <typeparam name="T">The <see langword="primitive"/> or <see langword="unmanaged"/> <see langword="struct"/> type to read</typeparam>
    /// <returns>A <see cref="Span{T}"/> where the length is the <paramref name="slice"/> length / <see langword="sizeof"/>(<typeparamref name="T"/>).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> ReadSpan<T>(this Span<byte> slice) where T : unmanaged
    {
        return MemoryMarshal.Cast<byte, T>(slice);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/> in the system endianness.
    /// </summary>
    /// <typeparam name="T">The <see langword="primitive"/> or <see langword="unmanaged"/> <see langword="struct"/> type to read</typeparam>
    /// <param name="count">The expected number of <typeparamref name="T"/> to read.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> ReadSpan<T>(this Span<byte> slice, int count) where T : unmanaged
    {
        return MemoryMarshal.Cast<byte, T>(slice)[..count];
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static Span<T> ReadSpan<T>(this Span<byte> slice, int count, Endianness endianness) where T : unmanaged
    {
        int size = sizeof(T);
        if (size > 1 && endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < count;) {
                slice[(size * i)..(size * (++i))].Reverse();
            }
        }

        return MemoryMarshal.Cast<byte, T>(slice);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <typeparamref name="R"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="R">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static Span<T> ReadSpan<T, R>(this Span<byte> slice, int count, Endianness endianness) where T : unmanaged where R : IStructReverser
    {
        int size = sizeof(T);
        if (size > 1 && endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < count;) {
                R.Reverse(slice[(size * i)..(size * (++i))]);
            }
        }

        return MemoryMarshal.Cast<byte, T>(slice);
    }
}
