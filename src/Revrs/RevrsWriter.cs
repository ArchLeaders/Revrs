using Revrs.Extensions;

namespace Revrs;

/// <summary>
/// Writes <see langword="primitive"/> and <see langword="struct"/> data types into a <see cref="Stream"/>, reversing the written values when required.
/// </summary>
public class RevrsWriter
{
    private readonly Stream _stream;
    private readonly Endianness _endianness;

    public long Position {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _stream.Position;
    }

    public RevrsWriter(in Stream stream, Endianness endianness)
    {
        if (!stream.CanWrite) {
            throw new InvalidOperationException("The input stream must be writable");
        }

        if (!stream.CanSeek) {
            throw new InvalidOperationException("The input stream must be seekable");
        }

        _stream = stream;
        _endianness = endianness;
    }

    /// <summary>
    /// Move the stream to an absolute <paramref name="position"/>.
    /// </summary>
    /// <param name="position">The absolute position to move reader to.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Seek(long position)
    {
        _stream.Seek(position, SeekOrigin.Begin);
    }

    /// <summary>
    /// Advance the stream position by a positive or negative <paramref name="size"/>.
    /// </summary>
    /// <param name="size">The positive or negative amount to move the reader position.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Move(long size)
    {
        _stream.Seek(size, SeekOrigin.Current);
    }

    /// <summary>
    /// Align the position <b>up (+)</b> to the provided <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Align(long size)
    {
        _stream.Seek(_stream.Position.AlignUp(size), SeekOrigin.Current);
    }

    /// <summary>
    /// Align the position <b>down (-)</b> to the provided <paramref name="size"/>.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AlignDown(long size)
    {
        _stream.Seek(_stream.Position.AlignDown(size), SeekOrigin.Current);
    }

    /// <summary>
    /// Write a <paramref name="buffer"/> of bytes and advance by the <paramref name="buffer"/> length.
    /// </summary>
    /// <param name="buffer">The buffer of bytes to write to the stream.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<byte> buffer)
    {
        _stream.Write(buffer);
    }

    /// <summary>
    /// Write the provided <paramref name="value"/> and advance the stream position by <see langword="sizeof"/>(<typeparamref name="T"/>).
    /// <para>
    /// <b>Note:</b> Writing types larger than 500 KB will allocate a buffer on the heap.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type to write.</typeparam>
    /// <param name="value">The <see langword="unmanaged"/> <see langword="primitive"/> value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write<T>(T value) where T : unmanaged
    {
        WriterExtensions.Write(_stream, value, _endianness);
    }

    /// <summary>
    /// Write the provided <paramref name="value"/> and advance the stream position by <see langword="sizeof"/>(<typeparamref name="T"/>).
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
    public unsafe void Write<T, R>(T value) where T : unmanaged where R : IStructReverser
    {
        WriterExtensions.Write<T, R>(_stream, value, _endianness);
    }
}
