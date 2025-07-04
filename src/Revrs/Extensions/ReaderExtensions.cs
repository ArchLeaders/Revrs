﻿using System.Runtime.InteropServices;

namespace Revrs.Extensions;

/// <summary>
/// Extension methods for reading unmanaged <see langword="primitive"/> and <see langword="struct"/> data types from a <see cref="Stream"/> or <see cref="Span{T}"/>, swapping the endianness when required.
/// </summary>
public static class ReaderExtensions
{
    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="stream"/> in the system endianness.
    /// </summary>
    /// <typeparam name="T">The primitive or struct type to read.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>A new instance of <typeparamref name="T"/> parsed from the <paramref name="stream"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T Read<T>(this Stream stream) where T : unmanaged
    {
        int size = sizeof(T);
        T result = default;
        stream.ReadExactly(new Span<byte>(&result, size));
        return result;
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="slice"/> in the system endianness.
    /// </summary>
    /// <typeparam name="T">The primitive or struct type to read.</typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Read<T>(this Span<byte> slice) where T : unmanaged
    {
        return ref MemoryMarshal.Cast<byte, T>(slice)[0];
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="slice"/> in the system endianness.
    /// </summary>
    /// <typeparam name="T">The primitive or struct type to read.</typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <returns>A copy of <typeparamref name="T"/> read from the provided <paramref name="slice"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Read<T>(this ReadOnlySpan<byte> slice) where T : unmanaged
    {
        return MemoryMarshal.Read<T>(slice);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="stream"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="stream"/>.</param>
    /// <returns>A new instance of <typeparamref name="T"/> parsed from the <paramref name="stream"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T Read<T>(this Stream stream, Endianness endianness) where T : unmanaged
    {
        int size = sizeof(T);
        T result = default;
        
        Span<byte> buffer = new(&result, size);
        stream.ReadExactly(buffer);

        if (size > 1 && endianness.IsNotSystemEndianness()) {
            buffer.Reverse();
        }

        return result;
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="slice"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read.</typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="slice"/>.</param>
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
    /// Read <typeparamref name="T"/> from the provided <paramref name="stream"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="stream"/>.</param>
    /// <returns>A new instance of <typeparamref name="T"/> parsed from the <paramref name="stream"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T Read<T, TReverser>(this Stream stream, Endianness endianness) where T : unmanaged where TReverser : IStructReverser
    {
        int size = sizeof(T);
        T result = default;
        
        Span<byte> buffer = new(&result, size);
        stream.ReadExactly(buffer);
        
        if (size > 1 && endianness.IsNotSystemEndianness()) {
            TReverser.Reverse(buffer);
        }

        return result;
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="stream"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="stream"/>.</param>
    /// <returns>A new instance of <typeparamref name="T"/> parsed from the <paramref name="stream"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe T ReadStruct<T>(this Stream stream, Endianness endianness) where T : unmanaged, IStructReverser
    {
        int size = sizeof(T);
        T result = default;
        
        Span<byte> buffer = new(&result, size);
        stream.ReadExactly(buffer);
        
        if (size > 1 && endianness.IsNotSystemEndianness()) {
            T.Reverse(buffer);
        }

        return result;
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="slice"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="slice"/>.</param>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Read<T, TReverser>(this Span<byte> slice, Endianness endianness) where T : unmanaged where TReverser : IStructReverser
    {
        if (slice.Length > 1 && endianness.IsNotSystemEndianness()) {
            TReverser.Reverse(slice);
        }

        return ref MemoryMarshal.Cast<byte, T>(slice)[0];
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="slice"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="slice"/>.</param>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T ReadStruct<T>(this Span<byte> slice, Endianness endianness) where T : unmanaged, IStructReverser
    {
        if (slice.Length > 1 && endianness.IsNotSystemEndianness()) {
            T.Reverse(slice);
        }

        return ref MemoryMarshal.Cast<byte, T>(slice)[0];
    }

    /// <summary>
    /// Read <typeparamref name="T"/>'s from the provided <paramref name="slice"/> in the system <see cref="Endianness"/> until the end of the <paramref name="slice"/>.
    /// </summary>
    /// <typeparam name="T">The <see langword="unmanaged"/> <see langword="primitive"/> or <see langword="struct"/> type to read</typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <returns>A new <see cref="Span{T}"/> where the length is the <paramref name="slice"/> length / <see langword="sizeof"/>(<typeparamref name="T"/>).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> ReadSpan<T>(this Span<byte> slice) where T : unmanaged
    {
        return MemoryMarshal.Cast<byte, T>(slice);
    }

    /// <summary>
    /// Read <typeparamref name="T"/>'s from the provided <paramref name="slice"/> in the system <see cref="Endianness"/> until the end of the <paramref name="slice"/>.
    /// </summary>
    /// <typeparam name="T">The <see langword="unmanaged"/> <see langword="primitive"/> or <see langword="struct"/> type to read</typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <returns>A new <see cref="Span{T}"/> where the length is the <paramref name="slice"/> length / <see langword="sizeof"/>(<typeparamref name="T"/>).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadSpan<T>(this ReadOnlySpan<byte> slice) where T : unmanaged
    {
        return MemoryMarshal.Cast<byte, T>(slice);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/> in the system endianness.
    /// </summary>
    /// <typeparam name="T">The <see langword="primitive"/> or <see langword="unmanaged"/> <see langword="struct"/> type to read.</typeparam>
    /// <param name="slice">The data to read from.</param>
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
    /// with this method, the entire <paramref name="slice"/> is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The <see langword="unmanaged"/> <see langword="primitive"/> type to read.</typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="slice"/>.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> ReadSpan<T>(this Span<byte> slice, int count, Endianness endianness) where T : unmanaged
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
    /// <typeparamref name="TReverser"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="slice"/>.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> ReadSpan<T, TReverser>(this Span<byte> slice, int count, Endianness endianness) where T : unmanaged where TReverser : IStructReverser
    {
        int size = sizeof(T);
        if (size > 1 && endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < count;) {
                TReverser.Reverse(slice[(size * i)..(size * ++i)]);
            }
        }

        return MemoryMarshal.Cast<byte, T>(slice);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <param name="slice">The data to read from.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="endianness">The <see langword="byte-order"/> to use when reading the <paramref name="slice"/>.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> ReadStructSpan<T>(this Span<byte> slice, int count, Endianness endianness) where T : unmanaged, IStructReverser
    {
        int size = sizeof(T);
        if (size > 1 && endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < count;) {
                T.Reverse(slice[(size * i)..(size * ++i)]);
            }
        }

        return MemoryMarshal.Cast<byte, T>(slice);
    }
}