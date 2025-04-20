using Revrs.Extensions;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Revrs;

/// <summary>
/// Writes <see langword="primitive"/> and <see langword="struct"/> data types into a <see cref="Stream"/>, reversing the written values when required.
/// </summary>
public readonly struct RevrsWriter
{
    private readonly Stream _stream;

    /// <summary>
    /// The target <see langword="byte-order"/> of the <see cref="RevrsWriter"/>.
    /// </summary>
    public readonly Endianness Endianness;

    /// <summary>
    /// The underlying <see cref="System.IO.Stream"/>.
    /// </summary>
    public readonly Stream Stream {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _stream;
    }

    /// <summary>
    /// The current position of the stream.
    /// </summary>
    public readonly long Position {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _stream.Position;
    }

    /// <summary>
    /// Create a new <see cref="RevrsWriter"/> from a <paramref name="stream"/> using the provided <paramref name="endianness"/>.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="endianness"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public RevrsWriter(in Stream stream, Endianness endianness)
    {
        if (!stream.CanWrite) {
            throw new InvalidOperationException("The input stream must be writable");
        }

        if (!stream.CanSeek) {
            throw new InvalidOperationException("The input stream must be seekable");
        }

        _stream = stream;
        Endianness = endianness;
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
    /// Flush the underlying <see cref="Stream"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Flush()
    {
        _stream.Flush();
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
    /// Aligns the position <b>up (+)</b> to the provided <paramref name="size"/> and ensures the padding is written.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AlignAtEnd(long size)
    {
        long alignValue = _stream.Position.AlignUp(size);
        if (alignValue == 0) return;
        
        _stream.Seek(alignValue - 1, SeekOrigin.Current);
        _stream.WriteByte(0);
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
    /// Aligns the position <b>down (-)</b> to the provided <paramref name="size"/> and ensures the padding is written.
    /// </summary>
    /// <param name="size"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AlignDownAtEnd(long size)
    {
        long alignValue = _stream.Position.AlignDown(size);
        if (alignValue == 0) return;
        
        _stream.Seek(alignValue - 1, SeekOrigin.Current);
        _stream.WriteByte(0);
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
    public void Write<T>(T value) where T : unmanaged
    {
        _stream.Write(value, Endianness);
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
    public void Write<T, R>(T value) where T : unmanaged where R : IStructReverser
    {
        _stream.Write<T, R>(value, Endianness);
    }

    /// <summary>
    /// Write the provided <see langword="managed"/> <see cref="string"/> as a UTF-8 unmanaged <see cref="byte"/>[] and advance the stream by the <paramref name="value"/> length.
    /// </summary>
    /// <param name="value">The managed <see cref="string"/> to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteStringUtf8(string value)
    {
        byte* ptr = Utf8StringMarshaller.ConvertToUnmanaged(value);
        Span<byte> buffer = new(ptr, Encoding.UTF8.GetByteCount(value));
        _stream.Write(buffer);
    }

    /// <summary>
    /// Write the provided <see langword="managed"/> <see cref="string"/> as a UTF-16 unmanaged <see cref="ushort"/>[] and advance the stream by the <paramref name="value"/> length.
    /// </summary>
    /// <param name="value">The managed <see cref="string"/> to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteStringUtf16(string value)
    {
        int length = Encoding.Unicode.GetByteCount(value) / 2;
        ushort* ptr = Utf16StringMarshaller.ConvertToUnmanaged(value);
        if (Endianness.IsNotSystemEndianness()) {
            for (int i = 0; i < length; i++) {
                ptr[i] = BinaryPrimitives.ReverseEndianness(ptr[i]);
            }
        }

        Span<ushort> buffer = new(ptr, length);
        _stream.Write(MemoryMarshal.Cast<ushort, byte>(buffer));
    }
}
