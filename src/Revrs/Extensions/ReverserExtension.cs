namespace Revrs.Extensions;

/// <summary>
/// Extension methods for reversing a span of unmanaged <see langword="primitive"/> and <see langword="struct"/> data types.
/// </summary>
public static class ReverserExtension
{
    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/>.
    /// <para>
    /// <b>Warning: </b> Only reverse <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire <paramref name="slice"/> is reversed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The <see langword="unmanaged"/> <see langword="primitive"/> type to reverse.</typeparam>
    /// <param name="slice">The data to reverse.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to reverse.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ReverseSpan<T>(this Span<byte> slice, int count) where T : unmanaged
    {
        int size = sizeof(T);
        if (size > 1) {
            for (int i = 0; i < count;) {
                slice[(size * i)..(size * (++i))].Reverse();
            }
        }
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/>.
    /// <para>
    /// <b>Warning: </b> Only reverse <a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive">primitive types</a>
    /// with this method, the entire <paramref name="slice"/> is reversed.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The <see langword="unmanaged"/> <see langword="primitive"/> type to reverse.</typeparam>
    /// <param name="slice">The data to reverse.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to reverse.</param>
    /// <param name="size">The size of the blocks to reverse.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseSpan<T>(this Span<byte> slice, int count, int size) where T : unmanaged
    {
        if (size > 1) {
            for (int i = 0; i < count;) {
                slice[(size * i)..(size * (++i))].Reverse();
            }
        }
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/>.
    /// <para>
    /// <typeparamref name="R"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse</typeparam>
    /// <typeparam name="R">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="slice">The data to reverse.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to reverse.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ReverseSpan<T, R>(this Span<byte> slice, int count) where T : unmanaged where R : IStructReverser
    {
        int size = sizeof(T);
        if (size > 1) {
            for (int i = 0; i < count;) {
                R.Reverse(slice[(size * i)..(size * (++i))]);
            }
        }
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/>.
    /// <para>
    /// will be used to reverse the buffer.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse</typeparam>
    /// <param name="slice">The data to reverse.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to reverse.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ReverseStructSpan<T>(this Span<byte> slice, int count) where T : unmanaged, IStructReverser
    {
        int size = sizeof(T);
        if (size > 1) {
            for (int i = 0; i < count;) {
                T.Reverse(slice[(size * i)..(size * (++i))]);
            }
        }
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/>.
    /// <para>
    /// <typeparamref name="R"/>, implementing <see cref="IStructReverser.Reverse(in Span{byte})"/>,
    /// will be used to reverse the buffer.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse</typeparam>
    /// <typeparam name="R">The <see cref="IStructReverser"/> to reverse <typeparamref name="T"/></typeparam>
    /// <param name="slice">The data to reverse.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to reverse.</param>
    /// <param name="size">The size of the blocks to reverse.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseSpan<T, R>(this Span<byte> slice, int count, int size) where T : unmanaged where R : IStructReverser
    {
        if (size > 1) {
            for (int i = 0; i < count;) {
                R.Reverse(slice[(size * i)..(size * (++i))]);
            }
        }
    }

    /// <summary>
    /// Reverse <paramref name="count"/> <typeparamref name="T"/>'s from the provided <paramref name="slice"/>.
    /// <para>
    /// will be used to reverse the buffer.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The struct to reverse</typeparam>
    /// <param name="slice">The data to reverse.</param>
    /// <param name="count">The number of <typeparamref name="T"/> to reverse.</param>
    /// <param name="size">The size of the blocks to reverse.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseStructSpan<T>(this Span<byte> slice, int count, int size) where T : unmanaged, IStructReverser
    {
        if (size > 1) {
            for (int i = 0; i < count;) {
                T.Reverse(slice[(size * i)..(size * (++i))]);
            }
        }
    }
}
