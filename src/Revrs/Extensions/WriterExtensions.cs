﻿using System.Runtime.InteropServices;

namespace Revrs.Extensions;

/// <summary>
/// Extension methods for writing <see langword="primitive"/> and <see langword="struct"/> data types into a <see cref="Stream"/>, reversing the written values when required.
/// </summary>
public static class WriterExtensions
{
    /// <summary>
    /// Write the provided <paramref name="value"/> into the <paramref name="stream"/> in the system endianness.
    /// <para>
    /// <b>Note:</b> Writing types larger than 500 KB will allocate a buffer on the heap.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type to write.</typeparam>
    /// <param name="value">The <see langword="unmanaged"/> <see langword="primitive"/> or <see langword="struct"/> value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Write<T>(this Stream stream, T value) where T : unmanaged
    {
        int size = sizeof(T);
        Span<byte> buffer = size <= 500_000
            ? stackalloc byte[size] : new byte[size];

        MemoryMarshal.Write(buffer, value);
        stream.Write(buffer);
    }

    /// <summary>
    /// Write the provided <paramref name="value"/> into the <paramref name="stream"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <b>Note:</b> Writing types larger than 500 KB will allocate a buffer on the heap.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type to write.</typeparam>
    /// <param name="value">The <see langword="unmanaged"/> <see langword="primitive"/> value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Write<T>(this Stream stream, T value, Endianness endianness) where T : unmanaged
    {
        int size = sizeof(T);
        Span<byte> buffer = size <= 500_000
            ? stackalloc byte[size] : new byte[size];

        MemoryMarshal.Write(buffer, value);
        if (endianness.IsNotSystemEndianness()) {
            buffer.Reverse();
        }

        stream.Write(buffer);
    }

    /// <summary>
    /// Write the provided <paramref name="value"/> into the <paramref name="stream"/> in the provided <paramref name="endianness"/>.
    /// <para>
    /// <typeparamref name="R"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the written value when endian swapping is required.
    /// </para>
    /// <para>
    /// <b>Note:</b> Writing types larger than 500 KB will allocate a buffer on the heap.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type to write.</typeparam>
    /// <typeparam name="R">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="value">The <see langword="unmanaged"/> <see langword="primitive"/> value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Write<T, R>(this Stream stream, T value, Endianness endianness) where T : unmanaged where R : IStructReverser
    {
        int size = sizeof(T);
        Span<byte> buffer = size <= 500_000
            ? stackalloc byte[size] : new byte[size];

        MemoryMarshal.Write(buffer, value);
        if (endianness.IsNotSystemEndianness()) {
            R.Reverse(buffer);
        }

        stream.Write(buffer);
    }
}
