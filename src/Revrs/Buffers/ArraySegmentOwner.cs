// Based on CommunityToolkit.HighPerformance.Buffers.SpanOwner<T>
// https://github.com/CommunityToolkit/dotnet/blob/main/src/CommunityToolkit.HighPerformance/Buffers/SpanOwner%7BT%7D.cs

using System.Buffers;
using System.Runtime.InteropServices;

namespace Revrs.Buffers;

/// <summary>
/// An <see langword="enum"/> that indicates a mode to use when allocating buffers.
/// </summary>
public enum AllocationMode
{
    /// <summary>
    /// The default allocation mode for pooled memory (rented buffers are not cleared).
    /// </summary>
    Default,

    /// <summary>
    /// Clear pooled buffers when renting them.
    /// </summary>
    Clear
}

/// <summary>
/// A stack-only type with the ability to rent a buffer of a specified length and provide a <see cref="ArraySegment{T}"/>.<br/>
/// <br/>
/// This type should always be used with a <see langword="using"/> block or expression.<br/>
/// Not doing so will cause the underlying buffer not to be returned to the shared pool.<br/>
/// <br/>
/// <b>Caution:</b> The provided <see cref="ArraySegment{T}"/> should never be stored, it will be discarded when the parent <see cref="ArraySegmentOwner{T}"/> is disposed.
/// </summary>
/// <typeparam name="T">The type of items to store in the current instance.</typeparam>
public readonly ref struct ArraySegmentOwner<T>
{
    /// <summary>
    /// The usable length within <see cref="_array"/>.
    /// </summary>
    private readonly int _length;

    /// <summary>
    /// The <see cref="ArrayPool{T}"/> instance used to rent <see cref="_array"/>.
    /// </summary>
    private readonly ArrayPool<T> _pool;

    /// <summary>
    /// The underlying <typeparamref name="T"/> array.
    /// </summary>
    private readonly T[] _array;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArraySegmentOwner{T}"/> struct with the specified parameters.
    /// </summary>
    /// <param name="length">The length of the new memory buffer to use.</param>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
    /// <param name="mode">Indicates the allocation mode to use for the new buffer to rent.</param>
    private ArraySegmentOwner(int length, ArrayPool<T> pool, AllocationMode mode)
    {
        _length = length;
        _pool = pool;
        _array = pool.Rent(length);

        if (mode == AllocationMode.Clear) {
            _array.AsSpan(0, length).Clear();
        }
    }

    /// <summary>
    /// Gets an empty <see cref="ArraySegmentOwner{T}"/> instance.
    /// </summary>
    public static ArraySegmentOwner<T> Empty {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(0, ArrayPool<T>.Shared, AllocationMode.Default);
    }

    /// <summary>
    /// Creates a new <see cref="ArraySegmentOwner{T}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="size">The length of the new memory buffer to use.</param>
    /// <returns>A <see cref="ArraySegmentOwner{T}"/> instance of the requested length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
    /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArraySegmentOwner<T> Allocate(int size)
    {
        return new(size, ArrayPool<T>.Shared, AllocationMode.Default);
    }

    /// <summary>
    /// Creates a new <see cref="ArraySegmentOwner{T}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="size">The length of the new memory buffer to use.</param>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
    /// <returns>A <see cref="ArraySegmentOwner{T}"/> instance of the requested length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
    /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArraySegmentOwner<T> Allocate(int size, ArrayPool<T> pool)
    {
        return new(size, pool, AllocationMode.Default);
    }

    /// <summary>
    /// Creates a new <see cref="ArraySegmentOwner{T}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="size">The length of the new memory buffer to use.</param>
    /// <param name="mode">Indicates the allocation mode to use for the new buffer to rent.</param>
    /// <returns>A <see cref="ArraySegmentOwner{T}"/> instance of the requested length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
    /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArraySegmentOwner<T> Allocate(int size, AllocationMode mode)
    {
        return new(size, ArrayPool<T>.Shared, mode);
    }

    /// <summary>
    /// Creates a new <see cref="ArraySegmentOwner{T}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="size">The length of the new memory buffer to use.</param>
    /// <param name="pool">The <see cref="ArrayPool{T}"/> instance to use.</param>
    /// <param name="mode">Indicates the allocation mode to use for the new buffer to rent.</param>
    /// <returns>A <see cref="ArraySegmentOwner{T}"/> instance of the requested length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size"/> is not valid.</exception>
    /// <remarks>This method is just a proxy for the <see langword="private"/> constructor, for clarity.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArraySegmentOwner<T> Allocate(int size, ArrayPool<T> pool, AllocationMode mode)
    {
        return new(size, pool, mode);
    }

    /// <summary>
    /// Gets the number of items in the current instance
    /// </summary>
    public int Length {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> wrapping the memory belonging to the current instance.
    /// </summary>
    public ArraySegment<T> Segment {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_array, 0, _length);
    }

    /// <summary>
    /// Returns a reference to the first element within the current instance, with no bounds check.
    /// </summary>
    /// <returns>A reference to the first element within the current instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T DangerousGetReference()
    {
        return ref MemoryMarshal.GetArrayDataReference(_array);
    }

    /// <summary>
    /// Gets an <see cref="ArraySegment{T}"/> instance wrapping the underlying <typeparamref name="T"/> array in use.
    /// </summary>
    /// <returns>An <see cref="ArraySegment{T}"/> instance wrapping the underlying <typeparamref name="T"/> array in use.</returns>
    /// <remarks>
    /// This method is meant to be used when working with APIs that only accept an array as input, and should be used with caution.
    /// In particular, the returned array is rented from an array pool, and it is responsibility of the caller to ensure that it's
    /// not used after the current <see cref="ArraySegmentOwner{T}"/> instance is disposed. Doing so is considered undefined behavior,
    /// as the same array might be in use within another <see cref="ArraySegmentOwner{T}"/> instance.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArraySegment<T> DangerousGetArray()
    {
        return new(_array!, 0, _length);
    }

    /// <summary>
    /// Implements the duck-typed <see cref="IDisposable.Dispose"/> method.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        _pool.Return(_array);
    }

    /// <summary>
    /// Returns the formatted type name and array length.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (typeof(T) == typeof(char) && _array is char[] chars) {
            return new(chars, 0, _length);
        }

        return $"Revrs.Buffers.ArraySegmentOwner<{typeof(T)}>[{_length}]";
    }
}
