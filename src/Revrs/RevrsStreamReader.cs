using Revrs.Extensions;

namespace Revrs;

/// <summary>
/// A delegate used to determine the BoM of data using a BigEndian <paramref name="reader"/>.
/// </summary>
public delegate Endianness GetByteOrderMarkFromStreamFunc(RevrsStreamReader reader);

/// <summary>
/// Reads <see langword="unmanaged"/> <see langword="primitive"/> and <see langword="struct"/> data types over a <see cref="System.IO.Stream"/>, reversing the bytes when required.
/// </summary>
public class RevrsStreamReader(Stream stream, Endianness endianness = Endianness.Big)
{
    /// <summary>
    /// The underlying <see cref="System.IO.Stream"/> to read from.
    /// </summary>
    public readonly Stream Stream = stream;

    /// <summary>
    /// The target <see langword="byte-order"/> of the <see cref="RevrsStreamReader"/>.
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public Endianness Endianness = endianness;

    /// <summary>
    /// The current position of the <see cref="RevrsStreamReader"/>.
    /// </summary>
    public long Position {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Stream.Position;
    }

    /// <summary>
    /// Get the length of the underlying <see cref="System.IO.Stream"/>.
    /// </summary>
    public long Length {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Stream.Length;
    }

    /// <summary>
    /// Create a new <see cref="RevrsStreamReader"/> using the system-native <see langword="byte-order"/>.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>A <see langword="new"/> <see cref="RevrsStreamReader"/> instatiated over the provided <paramref name="stream"/> using the system-native <see langword="byte-order"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RevrsStreamReader Native(Stream stream)
    {
        return new RevrsStreamReader(stream, BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big);
    }

    /// <summary>
    /// Create a new <see cref="RevrsStreamReader"/> using a predefined function to determine the byte order
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="getByteOrderMark">The function used to get the BoM</param>
    /// <returns>A <see langword="new"/> <see cref="RevrsStreamReader"/> instatiated over the provided <paramref name="stream"/> using the system-native <see langword="byte-order"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RevrsStreamReader Create(Stream stream, GetByteOrderMarkFromStreamFunc getByteOrderMark)
    {
        RevrsStreamReader reader = new(stream);
        reader.Endianness = getByteOrderMark(reader);
        reader.Stream.Seek(0, SeekOrigin.Begin);
        return reader;
    }

    /// <summary>
    /// Move the reader to an absolute <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The absolute position to move reader to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Seek(long position)
    {
        Stream.Seek(position, SeekOrigin.Begin);
    }

    /// <summary>
    /// Advance the reader position by a positive or negative <paramref name="size"/>.
    /// </summary>
    /// <param name="size">The positive or negative amount to move the reader position.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Move(long size)
    {
        Stream.Seek(size, SeekOrigin.Current);
    }

    /// <summary>
    /// Align the position <b>up (+)</b> to the provided <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Align(long size)
    {
        Stream.Seek(Position.AlignUp(size), SeekOrigin.Current);
    }

    /// <summary>
    /// Align the position <b>down (-)</b> to the provided <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AlignDown(long size)
    {
        Stream.Seek(Position.AlignDown(size), SeekOrigin.Current);
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
    public T Read<T>() where T : unmanaged => Stream.Read<T>(Endianness);

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
    public T Read<T>(long offset) where T : unmanaged
    {
        Stream.Seek(offset, SeekOrigin.Begin);
        return Stream.Read<T>(Endianness);
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
    public T Read<T, TReverser>() where T : unmanaged where TReverser : IStructReverser
        => Stream.Read<T, TReverser>(Endianness);

    /// <summary>
    /// Read <typeparamref name="T"/> from the readers current position and advance forward by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// will be used to reverse the buffer slice when endian swapping is required.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to read.</typeparam>
    /// <returns>A reference to <typeparamref name="T"/> over a span of data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ReadStruct<T>() where T : unmanaged, IStructReverser => Stream.ReadStruct<T>(Endianness);

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
    public T Read<T, TReverser>(long offset) where T : unmanaged where TReverser : IStructReverser
    {
        Stream.Seek(offset, SeekOrigin.Begin);
        return Stream.Read<T, TReverser>(Endianness);
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
    public T ReadStruct<T>(long offset) where T : unmanaged, IStructReverser
    {
        Stream.Seek(offset, SeekOrigin.Begin);
        return Stream.ReadStruct<T>(Endianness);
    }
}