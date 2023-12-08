using Revrs.Extensions;
using System.Runtime.InteropServices;

namespace Revrs;

/// <summary>
/// Reads <see langword="primitive"/> and <see langword="struct"/> data types over a <see cref="Span{T}"/> of bytes, reversing the underlying <see cref="Span{T}"/> when required.
/// </summary>
public ref struct RevrsReader(Span<byte> buffer, Endianness endianness = Endianness.Big)
{
    /// <summary>
    /// A <see cref="Span{T}"/> over the input buffer.
    /// </summary>
    public readonly Span<byte> Data = buffer;

    /// <summary>
    /// The target <see langword="byte-order"/> of the reader.
    /// </summary>
    public Endianness Endianness = endianness;

    /// <summary>
    /// The current position of the reader.
    /// </summary>
    public int Position = 0;

    /// <summary>
    /// Get the length of the underlying <see cref="Span{T}"/>.
    /// </summary>
    public readonly int Length {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Data.Length;
    }

    /// <summary>
    /// Create a new <see cref="RevrsReader"/> using the system-native <see langword="byte-order"/>.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns>A <see langword="new"/> <see cref="RevrsReader"/> instatiated over the provided <paramref name="buffer"/> using the system-native <see langword="byte-order"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RevrsReader Native(Span<byte> buffer)
    {
        return new(buffer, BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big);
    }

    /// <summary>
    /// Move the reader to an absolute <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The absolute position to move reader to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Seek(int position)
    {
        Position = position;
    }

    /// <summary>
    /// Advance the reader position by a positive or negative <paramref name="size"/>.
    /// </summary>
    /// <param name="size">The positive or negative amount to move the reader position.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Move(int size)
    {
        Position += size;
    }

    /// <summary>
    /// Align the position <b>up (+)</b> to the provided <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Align(int size)
    {
        Position += (size - Position % size) % size;
    }

    /// <summary>
    /// Align the position <b>down (-)</b> to the provided <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AlignDown(int size)
    {
        Position -= Position % size;
    }

    /// <summary>
    /// Get a span of bytes from the readers current position and advance forward by the provided <paramref name="length"/>.
    /// </summary>
    /// <param name="length"></param>
    /// <returns>A span of bytes where the size of the span is <paramref name="length"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> Read(int length)
    {
        return Data[Position..(Position += length)];
    }

    /// <summary>
    /// Get a span of bytes from the provided <paramref name="offset"/> and advance forward by the provided <paramref name="length"/>.
    /// </summary>
    /// <param name="offset">The absolue position to start reading the span.</param>
    /// <param name="length">The length of data to read.</param>
    /// <returns>A span of bytes where the size of the span is <paramref name="length"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> Read(int length, int offset)
    {
        return Data[offset..(Position = offset + length)];
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read.</typeparam>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T Read<T>() where T : unmanaged
    {
        Span<byte> slice = Data[Position..(Position += sizeof(T))];
        return ref ReaderExtensions.Read<T>(slice, Endianness);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read.</typeparam>
    /// <param name="offset">The absolue position to start reading the struct.</param>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T Read<T>(int offset) where T : unmanaged
    {
        Span<byte> slice = Data[offset..(Position = offset + sizeof(T))];
        return ref ReaderExtensions.Read<T>(slice, Endianness);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <typeparamref name="R"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="R">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T Read<T, R>() where T : unmanaged where R : IStructReverser
    {
        Span<byte> slice = Data[Position..(Position += sizeof(T))];
        return ref ReaderExtensions.Read<T, R>(slice, Endianness);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <typeparamref name="R"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="R">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="offset">The absolue position to start reading the struct.</param>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T Read<T, R>(int offset) where T : unmanaged where R : IStructReverser
    {
        Span<byte> slice = Data[offset..(Position = offset + sizeof(T))];
        return ref ReaderExtensions.Read<T, R>(slice, Endianness);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<T> ReadSpan<T>(int count) where T : unmanaged
    {
        int size = sizeof(T);
        Span<byte> slice = Data[Position..(Position += count * size)];
        return ReadSpan<T>(slice, count, size);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="offset">The absolue position to start reading the primitives.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<T> ReadSpan<T>(int count, int offset) where T : unmanaged
    {
        int size = sizeof(T);
        Span<byte> slice = Data[offset..(Position = offset + count * size)];
        return ReadSpan<T>(slice, count, size);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
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
    public unsafe Span<T> ReadSpan<T, R>(int count) where T : unmanaged where R : IStructReverser
    {
        int size = sizeof(T);
        Span<byte> slice = Data[Position..(Position += count * size)];
        return ReadSpan<T, R>(slice, count, size);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <typeparamref name="R"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="R">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="offset">The absolue position to start reading the structs.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<T> ReadSpan<T, R>(int count, int offset) where T : unmanaged where R : IStructReverser
    {
        int size = sizeof(T);
        Span<byte> slice = Data[offset..(Position = offset + count * size)];
        return ReadSpan<T, R>(slice, count, size);
    }

    /// <summary>
    /// Local function prefered over <see cref="ReadSpan{T}(Span{byte}, int, Endianness)"/> for performance.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly Span<T> ReadSpan<T>(Span<byte> slice, int count, int size) where T : unmanaged
    {
        if (size > 1 && Endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < count;) {
                slice[(size * i)..(size * (++i))].Reverse();
            }
        }

        return MemoryMarshal.Cast<byte, T>(slice);
    }

    /// <summary>
    /// Local function prefered over <see cref="ReadSpan{T, R}(Span{byte}, int, Endianness)"/> for performance.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly Span<T> ReadSpan<T, R>(Span<byte> slice, int count, int size) where T : unmanaged where R : IStructReverser
    {
        if (size > 1 && Endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < count;) {
                R.Reverse(slice[(size * i)..(size * (++i))]);
            }
        }

        return MemoryMarshal.Cast<byte, T>(slice);
    }
}
