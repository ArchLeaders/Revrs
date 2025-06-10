using Revrs.Extensions;
using System.Runtime.InteropServices;

namespace Revrs;

/// <summary>
/// A delegate used to determine the BoM of data using a BigEndian <paramref name="reader"/>.
/// </summary>
public delegate Endianness GetByteOrderMarkFunc(ref RevrsReader reader);

/// <summary>
/// Reads <see langword="unmanaged"/> <see langword="primitive"/> and <see langword="struct"/> data types over a <see cref="Span{T}"/> of bytes, reversing the underlying <see cref="Span{T}"/> when required.
/// </summary>
/// <param name="data">A <see cref="Span{T}"/> over the data buffer.</param>
/// <param name="endianness">The target <see langword="byte-order"/> of the <see cref="RevrsReader"/>.</param>
public ref struct RevrsReader(Span<byte> data, Endianness endianness = Endianness.Big)
{
    /// <summary>
    /// A <see cref="Span{T}"/> over the data buffer.
    /// </summary>
    public readonly Span<byte> Data = data;

    /// <summary>
    /// The target <see langword="byte-order"/> of the <see cref="RevrsReader"/>.
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public Endianness Endianness = endianness;

    /// <summary>
    /// The current position of the <see cref="RevrsReader"/>.
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
        return new RevrsReader(buffer, BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big);
    }

    /// <summary>
    /// Create a new <see cref="RevrsReader"/> using a predefined function to determine the byte order
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="getByteOrderMark">The function used to get the BoM</param>
    /// <returns>A <see langword="new"/> <see cref="RevrsReader"/> instatiated over the provided <paramref name="buffer"/> using the system-native <see langword="byte-order"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RevrsReader Create(Span<byte> buffer, GetByteOrderMarkFunc getByteOrderMark)
    {
        RevrsReader reader = new(buffer);
        Endianness bom = getByteOrderMark(ref reader);

        return bom switch {
            Endianness.Big => reader with { Position = 0 },
            Endianness.Little => new RevrsReader(buffer, Endianness.Little),
            _ => throw new InvalidDataException($"Invalid byte order mark: '{bom:x2}'")
        };
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
        Position += Position.AlignUp(size);
    }

    /// <summary>
    /// Align the position <b>down (-)</b> to the provided <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AlignDown(int size)
    {
        Position += Position.AlignDown(size);
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
        return ref slice.Read<T>(Endianness);
    }

    /// <summary>
    /// Reverse <typeparamref name="T"/> from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <b>Warning: </b> Only reverse <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to reverse.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Reverse<T>() where T : unmanaged
    {
        Data[Position..(Position += sizeof(T))].Reverse();
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
        return ref slice.Read<T>(Endianness);
    }

    /// <summary>
    /// Reverse <typeparamref name="T"/> from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to reverse.</typeparam>
    /// <param name="offset">The absolue position to start reversing the struct.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Reverse<T>(int offset) where T : unmanaged
    {
        Data[offset..(Position = offset + sizeof(T))].Reverse();
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see name="IReversablerseable.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read.</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/>.</typeparam>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T Read<T, TReverser>() where T : unmanaged where TReverser : IStructReverser
    {
        Span<byte> slice = Data[Position..(Position += sizeof(T))];
        return ref slice.Read<T, TReverser>(Endianness);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read.</typeparam>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T ReadStruct<T>() where T : unmanaged, IStructReverser
    {
        Span<byte> slice = Data[Position..(Position += sizeof(T))];
        return ref slice.ReadStruct<T>(Endianness);
    }

    /// <summary>
    /// Reverse <typeparamref name="T"/> from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see name="IReversablerseable.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse.</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/>.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Reverse<T, TReverser>() where T : unmanaged where TReverser : IStructReverser
    {
        TReverser.Reverse(Data[Position..(Position += sizeof(T))]);
    }

    /// <summary>
    /// Reverse <typeparamref name="T"/> from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// will be used to reverse the buffer slice.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReverseStruct<T>() where T : unmanaged, IStructReverser
    {
        T.Reverse(Data[Position..(Position += sizeof(T))]);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see name="IReversablerseable.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read.</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/>.</typeparam>
    /// <param name="offset">The absolue position to start reading the struct.</param>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T Read<T, TReverser>(int offset) where T : unmanaged where TReverser : IStructReverser
    {
        Span<byte> slice = Data[offset..(Position = offset + sizeof(T))];
        return ref slice.Read<T, TReverser>(Endianness);
    }

    /// <summary>
    /// Read <typeparamref name="T"/> from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read.</typeparam>
    /// <param name="offset">The absolue position to start reading the struct.</param>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ref T ReadStruct<T>(int offset) where T : unmanaged, IStructReverser
    {
        Span<byte> slice = Data[offset..(Position = offset + sizeof(T))];
        return ref slice.ReadStruct<T>(Endianness);
    }

    /// <summary>
    /// Reverse <typeparamref name="T"/> from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see name="IReversablerseable.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse.</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/>.</typeparam>
    /// <param name="offset">The absolue position to start reversing the struct.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Reverse<T, TReverser>(int offset) where T : unmanaged where TReverser : IStructReverser
    {
        TReverser.Reverse(Data[offset..(Position = offset + sizeof(T))]);
    }

    /// <summary>
    /// Reverse <typeparamref name="T"/> from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// will be used to reverse the buffer slice.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse.</typeparam>
    /// <param name="offset">The absolue position to start reversing the struct.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReverseStruct<T>(int offset) where T : unmanaged, IStructReverser
    {
        T.Reverse(Data[offset..(Position = offset + sizeof(T))]);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <b>Warning: </b> Only read <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to read.</typeparam>
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
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <b>Warning: </b> Only reverse <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to reverse.</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to reverse.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReverseSpan<T>(int count) where T : unmanaged
    {
        int size = sizeof(T);
        Data[Position..(Position += count * size)].ReverseSpan<T>(count, size);
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
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <b>Warning: </b> Only reverse <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire buffer slice is reversed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The primitive type to reverse.</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to reverse.</param>
    /// /// <param name="offset">The absolue position to start reversing the primitives.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReverseSpan<T>(int count, int offset) where T : unmanaged
    {
        int size = sizeof(T);
        Data[offset..(Position = offset + count * size)].ReverseSpan<T>(count, size);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see name="IReversablerseable.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read.</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/>.</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<T> ReadSpan<T, TReverser>(int count) where T : unmanaged where TReverser : IStructReverser
    {
        int size = sizeof(T);
        Span<byte> slice = Data[Position..(Position += count * size)];
        return ReadSpan<T, TReverser>(slice, count, size);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read.</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<T> ReadStructSpan<T>(int count) where T : unmanaged, IStructReverser
    {
        int size = sizeof(T);
        Span<byte> slice = Data[Position..(Position += count * size)];
        return ReadStructSpan<T>(slice, count, size);
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see name="IReversablerseable.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse.</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReverseSpan<T, TReverser>(int count) where T : unmanaged where TReverser : IStructReverser
    {
        int size = sizeof(T);
        Data[Position..(Position += count * size)].ReverseSpan<T, TReverser>(count, size);
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// will be used to reverse the buffer slice.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse.</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReverseStructSpan<T>(int count) where T : unmanaged, IStructReverser
    {
        int size = sizeof(T);
        Data[Position..(Position += count * size)].ReverseStructSpan<T>(count, size);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see name="IReversablerseable.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="offset">The absolue position to start reading the structs.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<T> ReadSpan<T, TReverser>(int count, int offset) where T : unmanaged where TReverser : IStructReverser
    {
        int size = sizeof(T);
        Span<byte> slice = Data[offset..(Position = offset + count * size)];
        return ReadSpan<T, TReverser>(slice, count, size);
    }

    /// <summary>
    /// Read <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="offset">The absolue position to start reading the structs.</param>
    /// <returns>A <see cref="Span{T}"/> where the length of the <see cref="Span{T}"/> is <paramref name="count"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe Span<T> ReadStructSpan<T>(int count, int offset) where T : unmanaged, IStructReverser
    {
        int size = sizeof(T);
        Span<byte> slice = Data[offset..(Position = offset + count * size)];
        return ReadStructSpan<T>(slice, count, size);
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// <typeparamref name="TReverser"/>, implementing <see name="IReversablerseable.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer slice.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse.</typeparam>
    /// <typeparam name="TReverser">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="offset">The absolue position to start reading the structs.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReverseSpan<T, TReverser>(int count, int offset) where T : unmanaged where TReverser : IStructReverser
    {
        int size = sizeof(T);
        Data[offset..(Position = offset + count * size)].ReverseSpan<T, TReverser>(count, size);
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="offset"/> and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>) * <paramref name="count"/>.
    /// <para>
    /// will be used to reverse the buffer slice.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse.</typeparam>
    /// <param name="count">The number of <typeparamref name="T"/> to read.</param>
    /// <param name="offset">The absolue position to start reading the structs.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ReverseStructSpan<T>(int count, int offset) where T : unmanaged, IStructReverser
    {
        int size = sizeof(T);
        Data[offset..(Position = offset + count * size)].ReverseStructSpan<T>(count, size);
    }

    /// <summary>
    /// Local function preferred over <see cref="ReaderExtensions.ReadSpan{T}(Span{byte},int,Revrs.Endianness)"/> for performance.
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
    /// Local function preferred over <see cref="ReaderExtensions.ReadSpan{T, R}(Span{byte},int,Revrs.Endianness)"/> for performance.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly Span<T> ReadSpan<T, TReverser>(Span<byte> slice, int count, int size) where T : unmanaged where TReverser : IStructReverser
    {
        if (size > 1 && Endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < count;) {
                TReverser.Reverse(slice[(size * i)..(size * (++i))]);
            }
        }

        return MemoryMarshal.Cast<byte, T>(slice);
    }

    /// <summary>
    /// Local function preferred over <see cref="ReaderExtensions.ReadSpan{T, R}(Span{byte},int,Revrs.Endianness)"/> for performance.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly Span<T> ReadStructSpan<T>(Span<byte> slice, int count, int size) where T : unmanaged, IStructReverser
    {
        if (size > 1 && Endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < count;) {
                T.Reverse(slice[(size * i)..(size * (++i))]);
            }
        }

        return MemoryMarshal.Cast<byte, T>(slice);
    }
}